// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Utility;

namespace xTask.Systems.File.Concrete;

/// <summary>
///  Simple implementation of a FileInfo wrapper
/// </summary>
public class FileInformation : FileSystemInformation, IFileInformation
{
    private readonly FileInfo _fileInfo;
    private string? _md5Hash;
    private IDirectoryInformation? _directoryInformation;

    public FileInformation(FileInfo fileInfo, IFileService fileService) : base(fileInfo, fileService)
    {
        _fileInfo = fileInfo;
    }

    public virtual ulong Length => (ulong)_fileInfo.Length;

    public virtual IDirectoryInformation Directory
        => _directoryInformation ??= new DirectoryInformation(_fileInfo.Directory!, FileService);

    public string MD5Hash => _md5Hash ??= GetHash();

    private string GetHash()
    {
        if (!Exists)
        {
            return string.Empty;
        }

        try
        {
            using Stream stream = FileService.CreateFileStream(Path);
            return Hash.GetHash(stream);
        }
        catch
        {
            return string.Empty;
        }
    }

    public override void Refresh()
    {
        _md5Hash = null;
        base.Refresh();
    }
}
