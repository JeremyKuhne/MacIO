// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;
using Task = xTask.Tasks.Task;

namespace MacIO
{
    public abstract class FileTask : Task
    {
        protected string GetFullTargetPath()
            => FileService.GetFullPath(Arguments.Target!, FileService.CurrentDirectory);

        protected IFileService FileService => this.GetRequiredService<IFileService>();

        protected IConsoleService ConsoleService => this.GetRequiredService<IConsoleService>();

        protected sealed override ExitCode ExecuteInternal()
        {
            ExitCode result = CheckPrerequisites();
            if (result != ExitCode.Success)
            {
                return result;
            }

            try
            {
                result = ExecuteFileTask();
            }
            catch (Exception e) when (e.IsIoException())
            {
                StatusLog.WriteLine(WriteStyle.Error, e.Message);
                return ExitCode.GeneralFailure;
            }

            return result;
        }

        protected virtual ExitCode CheckPrerequisites()
        {
            if (string.IsNullOrWhiteSpace(Arguments.Target))
            {
                StatusLog.WriteLine(WriteStyle.Error, "This command requires a target.");
                return ExitCode.InvalidArgument;
            }

            return ExitCode.Success;
        }

        protected abstract ExitCode ExecuteFileTask();

        protected static long ParseSize(string size)
        {
            long length;

            if (size.EndsWith("kb", StringComparison.OrdinalIgnoreCase))
            {
                length = long.Parse(size.AsSpan(0, size.Length - 2)) * 1024;
            }
            else if (size.EndsWith("mb", StringComparison.OrdinalIgnoreCase))
            {
                length = long.Parse(size.AsSpan(0, size.Length - 2)) * 1024 * 1024;
            }
            else if (size.EndsWith("gb", StringComparison.OrdinalIgnoreCase))
            {
                length = long.Parse(size.AsSpan(0, size.Length - 2)) * 1024 * 1024 * 1024;
            }
            else
            {
                length = long.Parse(size);
            }

            return length;
        }

        protected static string FormatSignature(ushort signature)
        {
            char c1 = (char)(signature >> 8);
            char c2 = (char)(signature & 0xff);
            return $"0x{signature:X} ({c1}{c2})";
        }

        protected static string FormatSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} byte";
            }
            else if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024.0:F1} KB";
            }
            else if (bytes < 1024 * 1024 * 1024)
            {
                return $"{bytes / (1024.0 * 1024):F1} MB";
            }
            else
            {
                return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
            }
        }
    }
}
