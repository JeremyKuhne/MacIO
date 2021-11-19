// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Path related helpers.
/// </summary>
/// <remarks>
///  Code in here should NOT touch actual IO.
/// </remarks>
public static class Paths
{
    private static readonly char[] s_DirectorySeparatorCharacters = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

    /// <summary>
    ///  Returns true if the given path has any of the specified extensions
    /// </summary>
    public static bool HasExtension(string path, params string[] extensions)
    {
        string pathExtension = GetExtension(path);
        return extensions.Any(extension => string.Equals(pathExtension, extension, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///  Attempt to retreive a file extension (with period), if any, from the given path or file name. Does not throw.
    /// </summary>
    public static string GetExtension(string pathOrFileName)
    {
        int extensionIndex = FindExtensionOffset(pathOrFileName);
        if (extensionIndex == -1)
        {
            // Nothing valid- return nothing
            return string.Empty;
        }
        else
        {
            return pathOrFileName[extensionIndex..];
        }
    }

    /// <summary>
    ///  Returns the index of the extension for the given path.  Does not validate paths in any way.
    /// </summary>
    /// <returns>The index of the period</returns>
    private static int FindExtensionOffset(string pathOrFileName)
    {
        if (string.IsNullOrEmpty(pathOrFileName)) { return -1; }

        int length = pathOrFileName.Length;

        // If are only one character long or we end with a period, return
        if ((length == 1) || pathOrFileName[length - 1] == '.')
        {
            return -1;
        }

        // Walk the string backwards looking for a period
        int index = length;
        while (--index >= 0)
        {
            char ch = pathOrFileName[index];
            if (ch == '.')
            {
                return index;
            }

            if (((ch == Path.DirectorySeparatorChar) || ch == ' ' || (ch == Path.AltDirectorySeparatorChar)) || (ch == Path.VolumeSeparatorChar))
            {
                // Found a space, directory or volume separator before a period
                // (this is something .NET gets wrong- extensions cannot have spaces in them)
                return -1;
            }
        }

        // No period at all
        return -1;
    }

    /// <summary>
    /// Finds the topmost directories for the specified paths that contain the paths passed in.
    /// </summary>
    public static IEnumerable<string> FindCommonRoots(IEnumerable<string> paths)
    {
        if (paths is null)
        {
            return Enumerable.Empty<string>();
        }

        HashSet<string> roots = new(StringComparer.OrdinalIgnoreCase);


        foreach (string path in paths)
        {
            if (string.IsNullOrWhiteSpace(path)) continue;

            string directory = Path.GetFileName(path);
            if (!roots.Contains(directory))
            {
                // Remove any directories that start with this directory
                if (roots.RemoveWhere(existingDirectory => existingDirectory.StartsWith(directory, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    // This is shorter than others that already exist, just add it
                    //
                    // (If we find C:\Foo\Bar\Bar for C:\Foo\ and we already haven't added C:\Foo\
                    // we can't have C:\ as this and the else statement pass below would have prevented
                    // this state.)
                    roots.Add(directory);
                }
                else
                {
                    // No matches, so we need to add if there isn't already a shorter path for this one
                    if (!roots.Any(root => directory.StartsWith(root, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Nothing starts our current directory, add it
                        roots.Add(directory);
                    }
                }
            }
        }

        return roots;
    }

    /// <summary>
    ///  Returns true if the path begins with a directory separator.
    /// </summary>
    public static bool BeginsWithDirectorySeparator(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return IsDirectorySeparator(path[0]);
    }

    /// <summary>
    ///  Returns true if the path ends in a directory separator.
    /// </summary>
    public static bool EndsInDirectorySeparator(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return IsDirectorySeparator(path[^1]);
    }

    /// <summary>
    ///  Returns true if the given character is a directory separator.
    /// </summary>
    public static bool IsDirectorySeparator(char character)
        => character == Path.DirectorySeparatorChar || character == Path.AltDirectorySeparatorChar;

    /// <summary>
    ///  Ensures that the specified path ends in a directory separator.
    /// </summary>
    /// <returns>The path with an appended directory separator if necessary.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if path is null.</exception>
    public static string AddTrailingSeparator(string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        return EndsInDirectorySeparator(path) ? path : path + Path.DirectorySeparatorChar;
    }

    /// <summary>
    ///  Ensures that the specified path does not end in a directory separator.
    /// </summary>
    /// <returns>The path with an appended directory separator if necessary.</returns>
    /// <exception cref="ArgumentNullException">Thrown if path is null.</exception>
    public static string RemoveTrailingSeparators(string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        return EndsInDirectorySeparator(path) ? path.TrimEnd(s_DirectorySeparatorCharacters) : path;
    }
}
