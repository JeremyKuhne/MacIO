// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class HeaderParseTests : IClassFixture<EmptyMac100MBHD>
{
    private readonly EmptyMac100MBHD _image;

    public HeaderParseTests(EmptyMac100MBHD image) => _image = image;

    [Fact]
    public void ParseBlockZero()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        BlockZero blockZero = new(stream);

        blockZero.Signature.Should().Be(0x4552);
        blockZero.BlockSize.Should().Be(512);
        blockZero.NumberOfBlocks.Should().Be(184320);

        var drivers = blockZero.DriverDescriptors;
        drivers.Count.Should().Be(1);

        drivers[0].StartingBlock.Should().Be(64);
        drivers[0].SizeInBlocks.Should().Be(19);
        drivers[0].OperatingSystemType.Should().Be(1);
    }

    [Fact]
    public void ParsePartitionMap()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        PartitionMapEntry entry = new(stream, index: 0);

        entry.Signature.Should().Be(0x504d);
        entry.MapEntries.Should().Be(3);
        entry.PhysicalStart.Should().Be(1);
        // Room for 63 partition entries
        entry.DataCount.Should().Be(63);
        entry.BlockCount.Should().Be(63);
        entry.Name.Should().Be("Apple");
        entry.TypeString.Should().Be("Apple_partition_map");
        entry.Type.Should().Be(PartitionType.PartitionMap);
        entry.ProcessorString.Should().Be(string.Empty);
        entry.Processor.Should().Be(ProcessorType.Unknown);
        entry.Status.Should().Be(PartitionStatus.Valid
            | PartitionStatus.Allocated
            | PartitionStatus.InUse
            | PartitionStatus.AllowsReading
            | PartitionStatus.AllowsWriting);
    }

    [Fact]
    public void ParseDriverPartition()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        PartitionMapEntry entry = new(stream, index: 1);

        entry.Signature.Should().Be(0x504d);
        entry.MapEntries.Should().Be(3);
        // At 0x8000
        entry.PhysicalStart.Should().Be(64);
        // 0x4000 bytes (out to 0xC000)
        entry.DataCount.Should().Be(32);
        entry.BlockCount.Should().Be(32);
        entry.Name.Should().Be("Macintosh");
        entry.TypeString.Should().Be("Apple_Driver43");
        entry.Type.Should().Be(PartitionType.DeviceDriver43);
        entry.ProcessorString.Should().Be("68000");
        entry.Processor.Should().Be(ProcessorType.Motorola68000);
        entry.BootChecksum.Should().Be(63012);
        entry.Status.Should().Be(PartitionStatus.Valid
            | PartitionStatus.Allocated
            | PartitionStatus.InUse
            | PartitionStatus.ValidBootInfo
            | PartitionStatus.AllowsReading
            | PartitionStatus.AllowsWriting
            | PartitionStatus.BootCodePositionIndependent);
    }

    [Fact]
    public void ParseHFSPartition()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        PartitionMapEntry entry = new(stream, index: 2);

        entry.Signature.Should().Be(0x504d);
        entry.MapEntries.Should().Be(3);

        // At 0xC000 (why do I see data two blocks later at 0xC400?)
        // Because there is no System startup information?? (Which is two blocks)
        entry.PhysicalStart.Should().Be(96);
        entry.DataCount.Should().Be(184224);
        entry.Name.Should().Be("MacOS");
        entry.TypeString.Should().Be("Apple_HFS");
        entry.Type.Should().Be(PartitionType.HierarchicalFileSystem);
        entry.ProcessorString.Should().Be(string.Empty);
        entry.Processor.Should().Be(ProcessorType.Unknown);
        entry.Status.Should().Be(PartitionStatus.Valid
            | PartitionStatus.Allocated
            | PartitionStatus.InUse
            | PartitionStatus.AllowsReading
            | PartitionStatus.AllowsWriting
            | PartitionStatus.Unused);
    }

    [Fact]
    public void ParseMasterDirectoryBlock()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        PartitionMapEntry entry = new(stream, index: 2);

        // Startup information is the first two blocks of the volume.
        MasterDirectoryBlock directory = new(stream, 512 * (entry.PhysicalStart + 2));
        directory.VolumeName.Should().Be("Blank 100M");
        directory.VolumeCreationTime.Should().Be(new DateTime(2021, 1, 8, 20, 1, 37, DateTimeKind.Local));
        directory.LastModificationTime.Should().Be(new DateTime(2021, 1, 8, 20, 12, 10, DateTimeKind.Local));
        directory.UnusedAllocationBlocks.Should().Be(60396);
        directory.VolumeAttributes.Should().Be(VolumeAttributes.SuccessfullyUnmounted);
        directory.VolumeBitmapFirstBlock.Should().Be(3);
        directory.FirstAllocationBlock.Should().Be(18);
        directory.AllocationBlockSize.Should().Be(512 * 3);
        directory.AllocationSearchBlock.Should().Be(1005);
        directory.CatalogFileSize.Should().Be(0x000b3a00);
        directory.CatalogClumpSize.Should().Be(0x000b3a00);
        directory.DefaultClumpSize.Should().Be(0x00001800);
        directory.ExtentsOverflowFileSize.Should().Be(0x000b3a00);
        directory.ExtentsClumpSize.Should().Be(0x000b3a00);
        directory.NextUnusedCatalogNodeId.Should().Be(22);
        directory.NumberOfFiles.Should().Be(2);
        directory.NumberOfRootFiles.Should().Be(2);
        directory.NumberOfDirectories.Should().Be(2);
        directory.NumberOfRootDirectories.Should().Be(2);
        directory.VolumeWriteCount.Should().Be(1430);
        directory.VolumeCacheSize.Should().Be(0);
        directory.VolumeBitmapCacheSize.Should().Be(0);
        directory.CommonVolumeCacheSize.Should().Be(0);
    }
}
