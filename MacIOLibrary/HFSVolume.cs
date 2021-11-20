// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public class HFSVolume : Record
{
    public HFSVolume(Stream stream, int partitionIndex, bool readOnly = true)
        : base(stream, readOnly)
    {
        BlockZero blockZero = new(stream);
        int blockSize = blockZero.BlockSize;

        if (blockSize == 0)
        {
            throw new InvalidOperationException("The image block size is zero.");
        }

        PartitionMapEntry partition = new(stream, partitionIndex, readOnly);
        if (partition.Type != PartitionType.HierarchicalFileSystem)
        {
            throw new InvalidDataException($"Partition {partitionIndex} is not an HFS partition.");
        }

        long position = partition.PhysicalStart * blockSize;

        BootBlockHeader = new(stream, position, readOnly);
        MasterDirectoryBlock = new(stream, position + 1024, readOnly);
    }

    public HFSVolume(Stream stream, long position = 0, bool readOnly = true)
        : base(stream, readOnly)
    {
        BootBlockHeader = new(stream, position, readOnly);
        MasterDirectoryBlock = new(stream, position + 1024, readOnly);
    }

    public BootBlockHeader BootBlockHeader { get; }
    public MasterDirectoryBlock MasterDirectoryBlock { get; }

    public override void Save()
    {
        BootBlockHeader.Save();
        MasterDirectoryBlock.Save();
    }
}
