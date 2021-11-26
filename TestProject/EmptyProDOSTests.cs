// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class EmptyProDOSTests : IClassFixture<EmptyProDOSImage>
{
    private readonly EmptyProDOSImage _image;

    public EmptyProDOSTests(EmptyProDOSImage image) => _image = image;

    [Fact]
    public void ParseVolume()
    {
        using FileStream stream = File.OpenRead(_image.ImagePath);
        ProDOSVolume volume = new(stream, position: 0, readOnly: true);
        volume.Name.Should().Be("MYPRODOS");
        volume.TotalBlocks.Should().Be(0xFFFE);
        volume.CreationTime.Should().Be(new DateTime(1956, 9, 7, 22, 59, 0, DateTimeKind.Local));
    }

    [Fact]
    public void ModifyVolumeInfo()
    {
        string newPath = Path.Join(Path.GetDirectoryName(_image.ImagePath), Path.GetRandomFileName());
        File.Copy(_image.ImagePath, newPath, overwrite: false);
        using FileStream stream = File.Open(newPath, FileMode.Open, FileAccess.ReadWrite);
        ProDOSVolume volume = new(stream, position: 0, readOnly: false);
        volume.Name = "PRODOS1";
        DateTime dateTime = new(2020, 2, 4, 18, 04, 0);
        volume.CreationTime = dateTime;
        volume.Save();
        volume = new(stream, position: 0, readOnly: false);
        volume.Name.Should().Be("PRODOS1");
        volume.CreationTime.Should().Be(dateTime);
    }
}
