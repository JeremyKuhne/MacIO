// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class CreateImageTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            string? sizeOption = Arguments.GetOption<string?>(StandardOptions.Size);
            if (string.IsNullOrEmpty(sizeOption))
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Need to specify a size (-size:).");
                return ExitCode.InvalidArgument;
            }

            long size;
            try
            {
                size = ParseSize(sizeOption);
            }
            catch (FormatException)
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Could not parse provided size.");
                return ExitCode.InvalidArgument;
            }

            uint prodosOption = Arguments.GetOption<uint?>("prodos") ?? 0;

            StatusLog.WriteLine($"Creating new {size} byte drive image...");

            string path = Arguments.Target ?? $"{FormatSize(size)} Mac Formatted.hda";
            path = FileService.GetFullPath(path, FileService.CurrentDirectory);
            using Stream stream = FileService.CreateFileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            DriveImage image = DriveImage.CreateEmptyImage(stream, size, prodosOption);

            StatusLog.WriteLine($"Finished writing image file \"{Path.GetFileName(path)}\".");

            return ExitCode.Success;
        }

        // Don't need a target.
        protected override ExitCode CheckPrerequisites() => ExitCode.Success;

        public override string? Summary => "Creates an empty image of the given size.";
        protected override string? GeneralHelp => Resources.HelpCreateImage;
        protected override string? OptionDetails => Resources.HelpCreateImageOptions;
    }
}
