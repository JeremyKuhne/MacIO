// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class EmptyHFSTests : IClassFixture<EmptyHFS5MBHD>
{
    private readonly EmptyHFS5MBHD _image;

    public EmptyHFSTests(EmptyHFS5MBHD image) => _image = image;

    [Fact]
    public void ParsePartition()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        DriveImage driveImage = new(stream);
        var map = driveImage.PartitionMap;
        var partitionEntry = map[2];
        partitionEntry.Type.Should().Be(PartitionType.HierarchicalFileSystem);

        long position = partitionEntry.PhysicalStart * driveImage.BlockZero.BlockSize;
        HFSVolume hfsPartition = new(stream, partitionIndex: 2);
        string volumeName = hfsPartition.MasterDirectoryBlock.VolumeName;
    }
}
