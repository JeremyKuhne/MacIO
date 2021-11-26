// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class CreateImageTests
{
    [Fact]
    public void CreateImage()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        DriveImage.CreateEmptyImage(stream, 5 * 1024 * 1024);

        ValidateCreateImage(stream);
    }

    [Fact]
    public void CreateImageTask()
    {
        using TestFileCleaner cleaner = new();
        string imagePath = Path.Join(cleaner.TempFolder, Path.GetRandomFileName());
        CreateImageTask task = new();
        TestInteraction interaction = new("createimage", imagePath, "-size:5MB");
        ExitCode exitCode = task.Execute(interaction);
        exitCode.Should().Be(ExitCode.Success);

        using FileStream stream = new(
            imagePath,
            FileMode.Open,
            FileAccess.Read);

        ValidateCreateImage(stream);
    }

    private void ValidateCreateImage(Stream stream)
    {
        DriveImage image = new(stream);
        var blockZero = image.BlockZero;
        blockZero.Signature.Should().Be(BlockZero.ValidSignature);
        blockZero.BlockSize.Should().Be(512);
        blockZero.NumberOfBlocks.Should().Be(10240);
        blockZero.DriverDescriptors.Count.Should().Be(1);
        blockZero.DriverDescriptors[0].StartingBlock.Should().Be(64);
        blockZero.DriverDescriptors[0].SizeInBlocks.Should().Be(19);
        blockZero.DriverDescriptors[0].OperatingSystemType.Should().Be(1);
        var map = image.PartitionMap;
        map.Count.Should().Be(3);
        var partitionEntry = map[0];
        partitionEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        partitionEntry.Type.Should().Be(PartitionType.PartitionMap);
        partitionEntry.MapEntries.Should().Be(3);
        partitionEntry.BlockCount.Should().Be(63);
        partitionEntry.Name.Should().Be("Apple");
        partitionEntry.PhysicalStart.Should().Be(1);
        var driverEntry = map[1];
        driverEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        driverEntry.Type.Should().Be(PartitionType.DeviceDriver43);
        driverEntry.Name.Should().Be("Macintosh");
        driverEntry.BootChecksum.Should().Be(63012);
        driverEntry.BootSize.Should().Be(9392);
        driverEntry.MapEntries.Should().Be(3);
        driverEntry.PhysicalStart.Should().Be(64);
        driverEntry.BlockCount.Should().Be(32);
        var freeEntry = map[2];
        freeEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        freeEntry.Type.Should().Be(PartitionType.Unused);
        freeEntry.Name.Should().Be("Unused1");
        freeEntry.PhysicalStart.Should().Be(96);
        freeEntry.BlockCount.Should().Be(10144);
        uint remainingBlocks = blockZero.NumberOfBlocks - partitionEntry.BlockCount - driverEntry.BlockCount - freeEntry.BlockCount;
        remainingBlocks.Should().Be(1); // For block zero itself
    }

    [Fact]
    public void CreateLargeImage()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        DriveImage.CreateEmptyImage(stream, 15L * 1024 * 1024 * 1024);

        DriveImage image = new(stream);
        var blockZero = image.BlockZero;
        blockZero.Signature.Should().Be(BlockZero.ValidSignature);
        blockZero.BlockSize.Should().Be(512);
        blockZero.NumberOfBlocks.Should().Be(31457280);
        blockZero.DriverDescriptors.Count.Should().Be(1);
        blockZero.DriverDescriptors[0].StartingBlock.Should().Be(64);
        blockZero.DriverDescriptors[0].SizeInBlocks.Should().Be(19);
        blockZero.DriverDescriptors[0].OperatingSystemType.Should().Be(1);
        var map = image.PartitionMap;
        map.Count.Should().Be(9);
        var partitionEntry = map[8];
        partitionEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        partitionEntry.Type.Should().Be(PartitionType.Unused);
        partitionEntry.MapEntries.Should().Be(9);
        // Don't want to be bigger than double the recommended HFS size
        partitionEntry.BlockCount.Should().BeLessThan((uint)(((2L * 1024 * 1024 * 1024) - (1 * 1024 * 1024)) / 512) * 2);
        partitionEntry.Name.Should().Be("Unused7");
    }

    [Fact]
    public void CreateImageWithOneProDOS()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        DriveImage.CreateEmptyImage(stream, 64 * 1024 * 1024, proDOSPartitions: 1);

        DriveImage image = new(stream);
        var blockZero = image.BlockZero;
        blockZero.Signature.Should().Be(BlockZero.ValidSignature);
        blockZero.BlockSize.Should().Be(512);
        blockZero.NumberOfBlocks.Should().Be(131072);
        blockZero.DriverDescriptors.Count.Should().Be(1);
        blockZero.DriverDescriptors[0].StartingBlock.Should().Be(64);
        blockZero.DriverDescriptors[0].SizeInBlocks.Should().Be(19);
        blockZero.DriverDescriptors[0].OperatingSystemType.Should().Be(1);

        var map = image.PartitionMap;
        map.Count.Should().Be(4);

        var partitionEntry = map[0];
        partitionEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        partitionEntry.Type.Should().Be(PartitionType.PartitionMap);
        partitionEntry.MapEntries.Should().Be(4);
        partitionEntry.BlockCount.Should().Be(63);
        partitionEntry.Name.Should().Be("Apple");
        partitionEntry.PhysicalStart.Should().Be(1);

        var driverEntry = map[1];
        driverEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        driverEntry.Type.Should().Be(PartitionType.DeviceDriver43);
        driverEntry.Name.Should().Be("Macintosh");
        driverEntry.BootChecksum.Should().Be(63012);
        driverEntry.BootSize.Should().Be(9392);
        driverEntry.MapEntries.Should().Be(4);
        driverEntry.PhysicalStart.Should().Be(64);
        driverEntry.BlockCount.Should().Be(32);

        var prodosEntry = map[2];
        prodosEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        prodosEntry.Type.Should().Be(PartitionType.ProDOSFileSystem);
        prodosEntry.MapEntries.Should().Be(4);
        prodosEntry.BlockCount.Should().Be(65534);
        prodosEntry.PhysicalStart.Should().Be(96);
        prodosEntry.Name.Should().Be("ProDOS1");
        prodosEntry.Status.Should().Be(PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
            | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting);

        ProDOSVolume volume = new(stream, partitionIndex: 2);
        volume.Name.Should().Be("ProDOS1");
        volume.TotalBlocks.Should().Be(65534);
        DateTime now = DateTime.Now;
        volume.CreationTime.Should().BeOnOrAfter(new DateTime(now.Year, now.Month, now.Day));

        var freeEntry = map[3];
        freeEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        freeEntry.Type.Should().Be(PartitionType.Unused);
        freeEntry.Name.Should().Be("Unused1");
        freeEntry.PhysicalStart.Should().Be(prodosEntry.PhysicalStart + prodosEntry.BlockCount);
        freeEntry.BlockCount.Should().Be(65442);
        uint remainingBlocks = blockZero.NumberOfBlocks - partitionEntry.BlockCount - driverEntry.BlockCount
            - prodosEntry.BlockCount - freeEntry.BlockCount;
        remainingBlocks.Should().Be(1); // For block zero itself
    }

    [Fact]
    public void CreateImageWithTwoProDOS()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        DriveImage.CreateEmptyImage(stream, 65 * 1024 * 1024, proDOSPartitions: 2);

        DriveImage image = new(stream);
        var blockZero = image.BlockZero;
        blockZero.Signature.Should().Be(BlockZero.ValidSignature);
        blockZero.BlockSize.Should().Be(512);
        blockZero.NumberOfBlocks.Should().Be(133120);
        blockZero.DriverDescriptors.Count.Should().Be(1);
        blockZero.DriverDescriptors[0].StartingBlock.Should().Be(64);
        blockZero.DriverDescriptors[0].SizeInBlocks.Should().Be(19);
        blockZero.DriverDescriptors[0].OperatingSystemType.Should().Be(1);

        var map = image.PartitionMap;
        map.Count.Should().Be(5);

        var partitionEntry = map[0];
        partitionEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        partitionEntry.Type.Should().Be(PartitionType.PartitionMap);
        partitionEntry.MapEntries.Should().Be(5);
        partitionEntry.BlockCount.Should().Be(63);
        partitionEntry.Name.Should().Be("Apple");
        partitionEntry.PhysicalStart.Should().Be(1);

        var driverEntry = map[1];
        driverEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        driverEntry.Type.Should().Be(PartitionType.DeviceDriver43);
        driverEntry.Name.Should().Be("Macintosh");
        driverEntry.BootChecksum.Should().Be(63012);
        driverEntry.BootSize.Should().Be(9392);
        driverEntry.MapEntries.Should().Be(5);
        driverEntry.PhysicalStart.Should().Be(64);
        driverEntry.BlockCount.Should().Be(32);

        var prodos1Entry = map[2];
        prodos1Entry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        prodos1Entry.Type.Should().Be(PartitionType.ProDOSFileSystem);
        prodos1Entry.MapEntries.Should().Be(5);
        prodos1Entry.BlockCount.Should().Be(65534);
        prodos1Entry.PhysicalStart.Should().Be(96);
        prodos1Entry.Name.Should().Be("ProDOS1");
        prodos1Entry.Status.Should().Be(PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
            | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting);

        ProDOSVolume volume = new(stream, partitionIndex: 2);
        volume.Name.Should().Be("ProDOS1");
        volume.TotalBlocks.Should().Be(65534);
        DateTime now = DateTime.Now;
        volume.CreationTime.Should().BeOnOrAfter(new DateTime(now.Year, now.Month, now.Day));

        var prodos2Entry = map[3];
        prodos2Entry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        prodos2Entry.Type.Should().Be(PartitionType.ProDOSFileSystem);
        prodos2Entry.MapEntries.Should().Be(5);
        prodos2Entry.BlockCount.Should().Be(65534);
        prodos2Entry.PhysicalStart.Should().Be(prodos1Entry.PhysicalStart + prodos1Entry.BlockCount);
        prodos2Entry.Name.Should().Be("ProDOS2");
        prodos2Entry.Status.Should().Be(PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
            | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting);

        volume = new(stream, partitionIndex: 3);
        volume.Name.Should().Be("ProDOS2");
        volume.TotalBlocks.Should().Be(65534);
        volume.CreationTime.Should().BeOnOrAfter(new DateTime(now.Year, now.Month, now.Day));

        var freeEntry = map[4];
        freeEntry.Signature.Should().Be(PartitionMapEntry.ValidSignature);
        freeEntry.Type.Should().Be(PartitionType.Unused);
        freeEntry.Name.Should().Be("Unused1");
        freeEntry.PhysicalStart.Should().Be(prodos2Entry.PhysicalStart + prodos2Entry.BlockCount);
        freeEntry.BlockCount.Should().Be(1956);
        uint remainingBlocks = blockZero.NumberOfBlocks - partitionEntry.BlockCount - driverEntry.BlockCount
            - prodos1Entry.BlockCount - prodos2Entry.BlockCount - freeEntry.BlockCount;
        remainingBlocks.Should().Be(1); // For block zero itself
    }

    [Fact]
    public void DeletePartition()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        var image = DriveImage.CreateEmptyImage(stream, 5 * 1024 * 1024);
        image.DeletePartition(index: 2);
    }
}
