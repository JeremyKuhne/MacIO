// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;
using xTask.Utility;

namespace MacIO
{
    public class ChecksumTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(GetFullTargetPath());
            ResultLog.WriteLine($"File hash: {Hash.GetHash(stream)}");
            return ExitCode.Success;
        }

        public override string? Summary => "Generates an MD5 checksum for a file.";
        protected override string? GeneralHelp => Resources.HelpChecksum;
    }
}
