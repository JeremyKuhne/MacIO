// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public class DriveImage
{
    private readonly Stream _stream;
    private BlockZero? _blockZero;

    public bool ReadOnly { get; }

    public DriveImage(Stream stream, bool readOnly = true)
    {
        _stream = stream;
        ReadOnly = readOnly;
    }

    public BlockZero BlockZero => _blockZero ??= new BlockZero(_stream, ReadOnly);

    public IReadOnlyList<PartitionMapEntry> PartitionMap
    {
        get => GetPartitionMap(readOnly: ReadOnly);
    }

    private PartitionMapEntry[] GetPartitionMap(bool readOnly = true)
    {
        PartitionMapEntry firstEntry = new(_stream, index: 0, readOnly);
        PartitionMapEntry[] entries = new PartitionMapEntry[firstEntry.MapEntries];
        entries[0] = firstEntry;

        for (int i = 1; i < entries.Length; i++)
        {
            entries[i] = new(_stream, i, readOnly);
        }

        return entries;
    }

    /// <summary>
    ///  Deletes the partition at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Partition map index of the partition that needs replaced.</param>
    /// <param name="eraseData">
    ///  True to erase the data in the partition in addition to removing the partition map entry.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  <paramref name="index"/> was not a valid index in the parition map.
    /// </exception>
    public void DeletePartition(int index, bool eraseData = false)
    {
        var map = GetPartitionMap(readOnly: false);
        if (index < 0 || index >= map.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        int blockSize = BlockZero.BlockSize;

        // Erase the old partition.
        if (eraseData)
        {
            var partition = map[index];
            if (partition.Type != PartitionType.PartitionMap)
            {
                _stream.Erase(partition.PhysicalStart * blockSize, partition.BlockCount * blockSize);
            }
        }

        // Erase the partition table.
        _stream.Erase(blockSize, blockSize * map.Length);

        int newIndex = 0;
        for (int i = 0; i < map.Length; i++)
        {
            if (i == index)
            {
                continue;
            }

            var partition = map[i];
            partition.MapEntries = (uint)map.Length - 1;
            partition.SetIndex(newIndex);
            partition.Save();
            newIndex++;
        }
    }

    // The recommended maximum size for HFS partitions is 2GB - 1MB
    // https://support.apple.com/kb/TA28860?locale=en_US
    // private const long RecommendedMaxParitionSize = (2L * 1024 * 1024 * 1024) - (1 * 1024 * 1024);

    public static DriveImage CreateEmptyImage(Stream stream, long sizeInBytes, params int[] partitionSizes)
    {
        // Pick the smallest block size that can cover the full size of the drive.
        // (In multiples of 512 bytes.)
        int blockSize = 512;
        while (blockSize * uint.MaxValue < sizeInBytes)
        {
            blockSize *= 2;
        }

        // Round up the requested size to the nearest block if needed.
        long partialBlock = sizeInBytes % blockSize;
        if (partialBlock > 0)
        {
            sizeInBytes += blockSize - partialBlock;
        }

        stream.SetLength(sizeInBytes);

        uint numberOfBlocks = checked((uint)(sizeInBytes / blockSize));

        // Set up the zero (boot) block.
        DriveImage image = new(stream, readOnly: false);
        var blockZero = image.BlockZero;
        blockZero.Signature = BlockZero.ValidSignature;
        blockZero.NumberOfBlocks = checked((uint)numberOfBlocks);
        blockZero.BlockSize = (ushort)blockSize;

        // No idea on why the next two reserved values are 1 on a Mac formatted drive.
        blockZero.DevId = 1;
        blockZero.DevType = 1;

        DriverIdentifier driver = new()
        {
            StartingBlock = 64,
            SizeInBlocks = 19,
            OperatingSystemType = 1
        };

        blockZero.DriverDescriptors = new[]{ driver };
        blockZero.Save();

        // Set up the partition map.
        PartitionMapEntry partitionEntry = new(stream, 0, readOnly: false)
        {
            Signature = PartitionMapEntry.ValidSignature,
            Type = PartitionType.PartitionMap,
            Name = "Apple",
            PhysicalStart = 1,
            // HD SC Setup 7.3.5 reserves 63 blocks for the partition table.
            BlockCount = 63,
            DataCount = 63,
            MapEntries = 3,
            Status = PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
                | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting
        };


        partitionEntry.Save();

        // This is the 7.3.5 driver.
        using var driverStream = typeof(DriveImage).Assembly.GetManifestResourceStream(
            "MacIOLibrary.Resources.Macintosh(DeviceDriver43-Motorola68000-1)");

        if (driverStream is null)
        {
            throw new InvalidOperationException("Could not load driver data.");
        }

        PartitionMapEntry driverEntry = new(stream, 1, readOnly: false)
        {
            Signature = PartitionMapEntry.ValidSignature,
            Type = PartitionType.DeviceDriver43,
            Name = "Macintosh",
            PhysicalStart = 64,
            // HD SC Setup 7.3.5 reserves 32 blocks for the driver.
            DataCount = 32,
            BlockCount = 32,
            MapEntries = 3,
            // No idea how this is generated/validated
            BootChecksum = 63012,
            Status = PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
                | PartitionStatus.ValidBootInfo | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting
                | PartitionStatus.BootCodePositionIndependent,
            BootSize = (uint)driverStream.Length,
            Processor = ProcessorType.Motorola68000
        };

        // The Apple drive setup software always sets these padding bits for the driver.
        // TODO: Find out what these are actually storing.
        var padding = driverEntry.PaddingBytes;
        padding[1] = 0x01;
        padding[2] = 0x06;
        padding[11] = 0x01;
        padding[13] = 0x07;

        driverEntry.Save();

        stream.Seek(64 * blockSize, SeekOrigin.Begin);
        driverStream.CopyTo(stream);

        uint usedBlocks = 1 + driverEntry.BlockCount + partitionEntry.BlockCount;
        uint remainingBlocks = numberOfBlocks - usedBlocks;

        // Set up "free" entries to fill the remaining space.
        PartitionMapEntry freeEntry = new(stream, 2, readOnly: false)
        {
            Signature = PartitionMapEntry.ValidSignature,
            Type = PartitionType.Unused,
            Name = "Unused",
            PhysicalStart = usedBlocks,
            DataCount = remainingBlocks,
            BlockCount = remainingBlocks,
            MapEntries = 3,
            Status = PartitionStatus.Valid | PartitionStatus.Allocated,
        };

        freeEntry.Save();

        return image;
    }
}
