// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class CreateImageTests
{
    [Fact]
    public void WhatsUp()
    {
        using FileStream stream = File.OpenRead(@"\\?\PhysicalDrive18");
        BlockZero blockZero = new(stream);
        long endPosition = blockZero.BlockSize * blockZero.NumberOfBlocks;

        byte[] buffer = new byte[1024];
        long read = 0;
        stream.Position = endPosition - 4096;
        do
        {
            read += stream.Read(buffer);
        }
        while (read < 4096);
        //long length = stream.Length;
        // 0x80070001
        // Going off the end, trying to get length...
    }

    [Fact]
    public void CreateImage()
    {
        using TestFileCleaner cleaner = new();
        using FileStream stream = new(
            Path.Join(cleaner.TempFolder, Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite);

        DriveImage.CreateEmptyImage(stream, 5 * 1024 * 1024);
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
