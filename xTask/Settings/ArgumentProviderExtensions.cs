// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Systems.File;
using xTask.Utility;

namespace xTask.Settings;

public static class ArgumentProviderExtensions
{
    /// <summary>
    ///  Gets target directory arguments or null if none specified
    /// </summary>
    public static string[] GetDirectories(this IArgumentProvider arguments, IFileService fileService)
        => SplitAndValidateDirectories(fileService, arguments.Targets);

    /// <summary>
    /// Returns directories from the specified argument or empty array if none specified
    /// </summary>
    public static string[] GetDirectoriesFromArgument(
        this IArgumentProvider arguments,
        IFileService fileService,
        params string[] optionAliases)
    {
        string? option = arguments.GetOption<string>(optionAliases);
        return option is null ? Array.Empty<string>() : SplitAndValidateDirectories(fileService, option);
    }

    /// <summary>
    ///  Returns files from the specified argument or empty array if none specified
    /// </summary>
    public static string[] GetFilesFromArgument(
        this IArgumentProvider arguments,
        IFileService fileService,
        params string[] optionAliases)
    {
        string? option = arguments.GetOption<string>(optionAliases);
        return option is null ? Array.Empty<string>() : SplitFiles(fileService, option);
    }

    /// <summary>
    ///  Returns files from the specified argument or empty array if none specified.
    /// </summary>
    public static string[] GetExtensionsFromArgument(this IArgumentProvider arguments, params string[] optionAliases)
    {
        string? option = arguments.GetOption<string>(optionAliases);
        return option is null ? Array.Empty<string>() : SplitExtensions(option);
    }

    private static string[] SplitAndValidateDirectories(IFileService fileService, string directoryList)
        => SplitAndValidateDirectories(fileService, new[] { directoryList });

    private static string[] SplitAndValidateDirectories(IFileService fileService, IReadOnlyList<string> directoryLists)
    {
        if (directoryLists is null || directoryLists.Count == 0 || directoryLists[0] == null)
        {
            return Array.Empty<string>();
        }

        List<string> directories = new();
        foreach (string directoryList in directoryLists)
        {
            foreach (string directory in directoryList.Split(';'))
            {
                string normalizedPath = fileService.GetFullPath(directory);
                if (!fileService.DirectoryExists(normalizedPath))
                {
                    throw new TaskArgumentException(Resources.ErrorDirectoryNotFound, directory);
                }
                directories.Add(normalizedPath);
            }
        }

        return directories.ToArray();
    }

    private static string[] SplitFiles(IFileService fileService, params string[] fileLists)
    {
        if (fileLists is null || fileLists.Length == 0 || fileLists[0] is null)
        {
            return Array.Empty<string>();
        }

        List<string> files = new();
        foreach (string fileList in fileLists)
        {
            foreach (string file in fileList.Split(';'))
            {
                files.Add(fileService.GetFullPath(file));
            }
        }

        return files.ToArray();
    }

    private static string[] SplitExtensions(params string[] extensionLists)
    {
        if (extensionLists is null || extensionLists.Length == 0 || extensionLists[0] is null)
        {
            return Array.Empty<string>();
        }

        List<string> extensions = new();

        foreach (string extensionList in extensionLists)
        {
            foreach (string extension in extensionList.Split(';'))
            {
                extensions.Add(extension.Trim().TrimStart('*'));
            }
        }

        return extensions.ToArray();
    }
}
