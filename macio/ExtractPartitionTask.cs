// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class ExtractPartitionTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream source = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(source);

            int index = Arguments.GetOption<int?>(StandardOptions.Index) ?? 0;

            var map = image.PartitionMap;
            if (index >= map.Count)
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Index {index} invalid. There are {map.Count} partitions.");
                return ExitCode.InvalidArgument;
            }

            var targets = Arguments.Targets;

            var partition = map[index];

            ushort blockSize = image.BlockZero.BlockSize;

            string size = FormatSize(partition.BlockCount * blockSize);

            string targetFile = targets.Count > 1 ? targets[1] : $"{partition.Name}({size} {partition.TypeString})";
            StatusLog.WriteLine($"Extracting {size} \"{partition.Name}\" partition...");

            using Stream target = FileService.CreateFileStream(targetFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            target.CopyFrom(source, partition.PhysicalStart * blockSize, partition.BlockCount * blockSize);

            ResultLog.WriteLine($"Extracted to: {targetFile}");

            return ExitCode.Success;
        }

        public override string? Summary => "Extracts the specified partition";
        protected override string? GeneralHelp =>  Resources.HelpExtractPartition;
        protected override string? OptionDetails => Resources.HelpExtractPartitionOptions;
    }
}
