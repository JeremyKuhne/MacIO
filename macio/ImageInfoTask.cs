// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class ImageInfoTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(stream);
            BlockZero zero = image.BlockZero;
            ResultLog.WriteLine($"{zero.NumberOfBlocks} blocks at {zero.BlockSize} bytes ({FormatSize((long)zero.NumberOfBlocks * zero.BlockSize)})");

            return ExitCode.Success;
        }

        public override string? Summary => "Lists the summary info for the image.";
    }
}
