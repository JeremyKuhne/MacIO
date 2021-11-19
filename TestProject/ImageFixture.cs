// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO.Compression;

namespace TestProject;

public abstract class ImageFixture : IDisposable
{
    private readonly TestFileCleaner _cleaner = new();

    public ImageFixture(string zipPath)
    {
        using ZipArchive archive = ZipFile.OpenRead(zipPath);
        ZipArchiveEntry? entry = archive.Entries[0];
        if (entry is null)
        {
            throw new ArgumentException("No entries in zip file.", nameof(zipPath));
        }

        ImagePath = Path.Join(_cleaner.TempFolder, entry.Name);
        entry.ExtractToFile(ImagePath);
    }

    public string ImagePath { get; private set; }

    public void Dispose()
    {
        _cleaner.Dispose();
        GC.SuppressFinalize(this);
    }
}
