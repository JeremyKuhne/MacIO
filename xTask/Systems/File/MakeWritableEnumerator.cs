// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO.Enumeration;

namespace xTask.Systems.File;

/// <summary>
///  Enumerates recursively through a directory trying to make all files writable.
/// </summary>
public class MakeWritableEnumerator : FileSystemEnumerator<string>
{
    public MakeWritableEnumerator(string directory)
        : base(directory, new EnumerationOptions
        {
            RecurseSubdirectories = true
        })
    { }

    protected override bool ShouldIncludeEntry(ref FileSystemEntry entry)
    {
        if (entry.Attributes.HasFlag(FileAttributes.ReadOnly))
        {
            // Make it writable
            entry.ToFileSystemInfo().Attributes = entry.Attributes & ~FileAttributes.ReadOnly;
        }

        return false;
    }

    protected override string TransformEntry(ref FileSystemEntry entry)
        => string.Empty;
}
