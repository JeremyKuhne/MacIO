// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Systems.File;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;

/// <summary>
///  Attempts to delete all specified paths when disposed.
/// </summary>
public class FileCleaner : IDisposable
{
    private const string xTaskFlagFileName = @"%xTaskFlagFile%";
    private readonly ConcurrentBag<string> _filesToClean = new();
    private readonly Stream _flagFile;
    private readonly string _rootTempFolder;
    private readonly IFileService _fileServiceProvider;

    private static readonly object s_CleanLock;

    static FileCleaner() => s_CleanLock = new object();

    /// <param name="tempRootDirectoryName">The subdirectory to use for temp files "MyApp"</param>
    public FileCleaner(string tempRootDirectoryName, IFileService fileService)
    {
        if (string.IsNullOrWhiteSpace(tempRootDirectoryName))
            throw new ArgumentNullException(nameof(tempRootDirectoryName));
        if (fileService is null)
            throw new ArgumentNullException(nameof(fileService));

        _fileServiceProvider = fileService;
        _rootTempFolder = Path.Join(Path.GetTempPath(), tempRootDirectoryName);
        TempFolder = Path.Join(_rootTempFolder, Path.GetRandomFileName());
        string flagFile = Path.Join(TempFolder, xTaskFlagFileName);

        lock (s_CleanLock)
        {
            // Make sure we fully lock the directory before allowing cleaning
            _fileServiceProvider.CreateDirectory(TempFolder);

            // Create a flag file and leave it open- this way we can track and clean abandoned (crashed/terminated) processes
            _flagFile = _fileServiceProvider.CreateFileStream(flagFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

            using StreamWriter writer = new(_flagFile, leaveOpen: true);
            writer.WriteLine(Resources.FlagFileContent);
            writer.Flush();
        }
    }

    public string TempFolder { [DebuggerStepThrough] get; private set; }

    public void TrackFile(string path)
    {
        if (!string.IsNullOrEmpty(path) && !path.StartsWith(TempFolder, StringComparison.OrdinalIgnoreCase))
        {
            _filesToClean.Add(path);
        }
    }

    protected virtual void CleanOrphanedTempFolders()
    {
        // Clean up orphaned temp folders

        if (_fileServiceProvider.GetPathInfo(_rootTempFolder) is not IDirectoryInformation rootInfo)
        {
            return;
        }

        try
        {
            var flagFiles =
                from directory in rootInfo.EnumerateDirectories()
                from file in directory.EnumerateFiles(xTaskFlagFileName)
                select new { Directory = directory.Path, File = file.Path };

            foreach (var flagFile in flagFiles.ToArray())
            {
                try
                {
                    // If we can't delete the flag file (open handle) we'll throw and move on
                    _fileServiceProvider.DeleteFile(flagFile.File);
                    _fileServiceProvider.DeleteDirectory(flagFile.Directory, deleteChildren: true);
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception)
        {
            // Ignoring orphan cleanup errors as the DotNet file service chokes on long paths
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual bool ThrowOnCleanSelf => false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        lock (s_CleanLock)
        {
            _flagFile.Dispose();

            // Delete our own temp folder
            try
            {
                _fileServiceProvider.DeleteDirectory(TempFolder, deleteChildren: true);
            }
            catch (Exception)
            {
                if (ThrowOnCleanSelf) throw;
            }

            // Clean any loose files we're tracking
            foreach (string file in _filesToClean.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(file)) { continue; }

                try
                {
                    _fileServiceProvider.DeleteFile(file);
                }
                catch (Exception)
                {
                    if (ThrowOnCleanSelf) throw;
                }
            }

            CleanOrphanedTempFolders();
        }
    }
}
