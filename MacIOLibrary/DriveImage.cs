// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO.Compression;

namespace MacIO;

public class DriveImage
{
    // The recommended maximum size for HFS partitions is 2GB - 1MB
    // https://support.apple.com/kb/TA28860?locale=en_US
    private const long RecommendedMaxParitionSize = (2L * 1024 * 1024 * 1024) - (1L * 1024 * 1024);

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

    private static uint BlockCount(long bytes, long blockSize)
        => checked((uint)((bytes / blockSize) + (bytes % blockSize > 0 ? 1u : 0)));

    public static DriveImage CreateEmptyImage(Stream stream, long sizeInBytes, uint proDOSPartitions = 0)
    {
        if (sizeInBytes < 131072)
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Image must be at least 128K");

        // Pick the smallest block size that can cover the full size of the drive.
        // (In multiples of 512 bytes.)
        long blockSize = 512;
        while (blockSize * uint.MaxValue < sizeInBytes)
        {
            blockSize *= 2;
        }

        uint numberOfBlocks = BlockCount(sizeInBytes, blockSize);
        sizeInBytes = checked(blockSize * numberOfBlocks);
        stream.SetLength(sizeInBytes);

        // Blocks used for block zero and the partition table.
        const ushort ReservedBlocks = 64;
        const ushort DriverPartitionBlocks = 32;
        const ushort MaxPartitionEntries = 63;

        // Grab the blank ProDOS image if we need it and calculate sizes.
        bool hasProDOSParitions = proDOSPartitions > 0;
        using Stream? proDOSZip = hasProDOSParitions
            ? typeof(DriveImage).Assembly.GetManifestResourceStream("MacIOLibrary.Resources.ProDOS(32.0 MB Apple_PRODOS).zip")
            : null;

        Debug.Assert(!hasProDOSParitions || proDOSZip is not null);
        var zipEntry = proDOSZip is not null ? new ZipArchive(proDOSZip).Entries[0] : null;
        long proDOSLength = zipEntry?.Length ?? 0;
        uint proDOSBlockCount = BlockCount(proDOSLength, blockSize);
        uint totalProDOSBlocks = checked(proDOSBlockCount * proDOSPartitions);

        // Figure out the number of free partitions to create.
        uint remainingBlocksForBlankPartitions = numberOfBlocks - (ReservedBlocks + DriverPartitionBlocks);
        if (remainingBlocksForBlankPartitions < totalProDOSBlocks)
        {
            throw new ArgumentOutOfRangeException(nameof(proDOSPartitions), "Not enough space for requested ProDOS partitions.");
        }
        remainingBlocksForBlankPartitions -= totalProDOSBlocks;

        uint recommendedPartitionSizeInBlocks = BlockCount(RecommendedMaxParitionSize, blockSize);
        uint blankPartitions = remainingBlocksForBlankPartitions < 1
            ? 0
            : Math.Max(1, remainingBlocksForBlankPartitions / recommendedPartitionSizeInBlocks);

        // Plus 2 for the partition map entry and driver partition
        uint totalPartitions = blankPartitions + proDOSPartitions + 2;

        if (totalPartitions > MaxPartitionEntries)
        {
            throw new InvalidOperationException("Need more partition entries than are available.");
        }

        // Set up the zero (boot) block.
        DriveImage image = new(stream, readOnly: false);
        var blockZero = image.BlockZero;
        blockZero.Signature = BlockZero.ValidSignature;
        blockZero.NumberOfBlocks = numberOfBlocks;
        blockZero.BlockSize = (ushort)blockSize;

        // No idea on why the next two reserved values are 1 on a Mac formatted drive.
        blockZero.DevId = 1;
        blockZero.DevType = 1;

        DriverIdentifier driver = new()
        {
            StartingBlock = ReservedBlocks,
            SizeInBlocks = 19,
            OperatingSystemType = 1
        };

        blockZero.DriverDescriptors = new[]{ driver };
        blockZero.Save();

        // Set up the partition map.
        int partitionIndex = 0;
        PartitionMapEntry partitionEntry = new(stream, partitionIndex++, readOnly: false)
        {
            Signature = PartitionMapEntry.ValidSignature,
            Type = PartitionType.PartitionMap,
            Name = "Apple",
            PhysicalStart = 1,
            // HD SC Setup 7.3.5 reserves 63 blocks for the partition table.
            BlockCount = 63,
            DataCount = 63,
            MapEntries = totalPartitions,
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

        PartitionMapEntry driverEntry = new(stream, partitionIndex++, readOnly: false)
        {
            Signature = PartitionMapEntry.ValidSignature,
            Type = PartitionType.DeviceDriver43,
            Name = "Macintosh",
            PhysicalStart = ReservedBlocks,
            // HD SC Setup 7.3.5 reserves 32 blocks for the driver.
            DataCount = DriverPartitionBlocks,
            BlockCount = DriverPartitionBlocks,
            MapEntries = totalPartitions,
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

        stream.Seek((long)(ReservedBlocks * blockSize), SeekOrigin.Begin);
        driverStream.CopyTo(stream);

        Debug.Assert(1 + driverEntry.BlockCount + partitionEntry.BlockCount == ReservedBlocks + DriverPartitionBlocks);
        uint usedBlocks = 1 + driverEntry.BlockCount + partitionEntry.BlockCount;
        uint remainingBlocks = numberOfBlocks - usedBlocks;

        // Create our ProDOS partitions if needed
        for (int i = 0; i < proDOSPartitions; i++)
        {
            // You can't seek a stream from a ZipEntry, it must be reopened.
            Stream proDOSStream = zipEntry!.Open();
            string name = $"ProDOS{i + 1}";
            PartitionMapEntry proDOSEntry = new(stream, partitionIndex++, readOnly: false)
            {
                Signature = PartitionMapEntry.ValidSignature,
                Type = PartitionType.ProDOSFileSystem,
                Name = name,
                PhysicalStart = usedBlocks,
                DataCount = proDOSBlockCount,
                BlockCount = proDOSBlockCount,
                MapEntries = totalPartitions,
                Status = PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
                    | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting,
            };

            long position = usedBlocks * blockSize;
            stream.Position = position;
            proDOSStream.CopyTo(stream);

            ProDOSVolume proDOSVolume = new(stream, position, readOnly: false);
            proDOSVolume.Name = name;
            proDOSVolume.CreationTime = DateTime.Now;

            usedBlocks += proDOSBlockCount;
            remainingBlocks -= proDOSBlockCount;

            proDOSVolume.Save();
            proDOSEntry.Save();
        }

        // Set up "free" entries to fill the remaining space.
        uint blankPartitionSize = recommendedPartitionSizeInBlocks;
        for (int i = 0; i < blankPartitions; i++)
        {
            if (i == blankPartitions - 1)
            {
                blankPartitionSize = remainingBlocks;
            }

            PartitionMapEntry freeEntry = new(stream, partitionIndex++, readOnly: false)
            {
                Signature = PartitionMapEntry.ValidSignature,
                Type = PartitionType.Unused,
                Name = $"Unused{i + 1}",
                PhysicalStart = usedBlocks,
                DataCount = blankPartitionSize,
                BlockCount = blankPartitionSize,
                MapEntries = totalPartitions,
                Status = PartitionStatus.Valid | PartitionStatus.Allocated,
            };

            usedBlocks += blankPartitionSize;
            remainingBlocks -= blankPartitionSize;
            freeEntry.Save();
        }

        Debug.Assert(remainingBlocks == 0);

        return image;
    }
}
