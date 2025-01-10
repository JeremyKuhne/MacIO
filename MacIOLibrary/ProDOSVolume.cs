// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace MacIO;

public partial class ProDOSVolume : Record
{
    private readonly byte[] _bootLoaderImage = new byte[512 * 2];
    private readonly byte[] _volumeDirectoryKeyBlock = new byte[512];
    private ref VolumeHeader VolumeData
        => ref Unsafe.As<byte, VolumeHeader>(ref Unsafe.AsRef(in _volumeDirectoryKeyBlock[4]));

    private readonly long _position;

    public ProDOSVolume(Stream stream, int partitionIndex, bool readOnly = true)
        : base(stream, readOnly)
    {
        BlockZero blockZero = new(stream);
        int blockSize = blockZero.BlockSize;

        if (blockSize == 0)
        {
            throw new InvalidOperationException("The image block size is zero.");
        }

        PartitionMapEntry partition = new(stream, partitionIndex, readOnly);
        if (partition.Type != PartitionType.ProDOSFileSystem)
        {
            throw new InvalidDataException($"Partition {partitionIndex} is not a ProDOS partition.");
        }

        _position = partition.PhysicalStart * blockSize;

        Stream.Seek(_position, SeekOrigin.Begin);
        Stream.Read(_bootLoaderImage);
        Stream.Read(_volumeDirectoryKeyBlock);
    }

    public ProDOSVolume(Stream stream, long position = 0, bool readOnly = true)
        : base(stream, readOnly)
    {
        _position = position;
        Stream.Seek(_position, SeekOrigin.Begin);
        Stream.Read(_bootLoaderImage);
        Stream.Read(_volumeDirectoryKeyBlock);
    }

    public override void Save()
    {
        Stream.Seek(_position, SeekOrigin.Begin);
        Stream.Write(_bootLoaderImage);
        Stream.Write(_volumeDirectoryKeyBlock);
    }

    public string Name
    {
        get => VolumeData.type_and_name.Name;
        set => VolumeData.type_and_name.Name = value;
    }

    public DateTime CreationTime
    {
        get => Conversion.FromProDOSDate(VolumeData.creation_date, VolumeData.creation_time);
        set
        {
            (ushort date, ushort time) = Conversion.ToProDOSDate(value);
            VolumeData.creation_date = date;
            VolumeData.creation_time = time;
        }
    }

    public ushort TotalBlocks
    {
        get => VolumeData.total_blocks;
    }

    public ushort FileCount
    {
        get => VolumeData.file_count;
    }
}

