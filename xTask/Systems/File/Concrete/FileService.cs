// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Systems.File.Concrete;

/// <summary>
///  File service that exclusively uses the .NET IO implementation to back it
/// </summary>
public class FileService : IFileService
{
    public string CurrentDirectory
    {
        get => Environment.CurrentDirectory;
        set => Environment.CurrentDirectory = value;
    }

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public Stream CreateFileStream(
        string path,
        FileMode mode = FileMode.Open,
        FileAccess access = FileAccess.Read,
        FileShare share = FileShare.ReadWrite)
        => new FileStream(path, mode, access, share);

    public void DeleteDirectory(string path, bool deleteChildren = false)
        => Directory.Delete(path, recursive: deleteChildren);

    public void DeleteFile(string path) => System.IO.File.Delete(path);

    public IFileSystemInformation GetPathInfo(string path)
    {
        FileAttributes attributes = System.IO.File.GetAttributes(path);
        return attributes.HasFlag(FileAttributes.Directory)
            ? new DirectoryInformation(new(path), this)
            : new FileInformation(new(path), this);
    }

    public string GetFullPath(string path, string? basePath = null)
    {
        if (basePath is null || Path.IsPathFullyQualified(path))
        {
            // Fixed, or we don't have a base path
            return Path.GetFullPath(path);
        }
        else
        {
            return Path.GetFullPath(path, basePath);
        }
    }

    public FileAttributes GetAttributes(string path) => System.IO.File.GetAttributes(path);

    public void SetAttributes(string path, FileAttributes attributes) => System.IO.File.SetAttributes(path, attributes);

    public void CopyFile(string existingPath, string newPath, bool overwrite = false)
        => System.IO.File.Copy(existingPath, newPath, overwrite);
}
