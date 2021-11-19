// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class DeletePartitionTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(
                GetFullTargetPath(),
                access: FileAccess.ReadWrite,
                share: FileShare.None);

            DriveImage image = new(stream, readOnly: false);

            var entries = image.PartitionMap;
            if (entries.Count == 0)
            {
                StatusLog.WriteLine(WriteStyle.Error, "No partitions found.");
                return ExitCode.FileNotFound;
            }

            int? indexOption = Arguments.GetOption<int?>(StandardOptions.Index);
            int index = 0;

            if (indexOption.HasValue)
            {
                index = indexOption.Value;
                if (index < 0 || index >= entries.Count)
                {
                    StatusLog.WriteLine(WriteStyle.Error, $"Invalid index {index}. Entry count is {entries.Count}.");
                    return ExitCode.InvalidArgument;
                }
            }

            var entry = entries[index];

            if (!ConsoleService.QueryYesNo(
                $"Delete {FormatSize((long)entry.BlockCount * 512)} \"{entry.Name}\" partition {index}?"))
            {
                return ExitCode.Canceled;
            }

            image.DeletePartition(index, eraseData: true);

            return ExitCode.Success;
        }

        public override string? Summary => "Deletes the specified partition.";
    }
}
