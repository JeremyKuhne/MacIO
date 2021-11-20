// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class ReplacePartitionTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            var targets = Arguments.Targets;
            if (targets.Count < 2)
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Need a target image and a source partition file.");
                return ExitCode.InvalidArgument;
            }

            using Stream source = FileService.CreateFileStream(targets[1]);

            using Stream target = FileService.CreateFileStream(
                GetFullTargetPath(),
                access: FileAccess.ReadWrite,
                share: FileShare.Read);

            DriveImage image = new(target, readOnly: false);

            int index = Arguments.GetOption<int?>(StandardOptions.Index) ?? 0;

            var map = image.PartitionMap;
            if (index >= map.Count)
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Index {index} invalid. There are {map.Count} partitions.");
                return ExitCode.InvalidArgument;
            }

            int blockSize = image.BlockZero.BlockSize;

            var partition = map[index];
            long startingPosition = partition.PhysicalStart * blockSize;
            long partitionLength = partition.BlockCount * blockSize;

            if (partitionLength < source.Length)
            {
                StatusLog.WriteLine(
                    WriteStyle.Error,
                    $"Existing paritition too small ({FormatSize(partitionLength)}). Need {FormatSize(source.Length)}.");
                return ExitCode.InvalidData;
            }

            if (partitionLength != source.Length)
            {
                if (!ConsoleService.QueryYesNo(
                    $"Replacement file {source.Length}({FormatSize(source.Length)}) is smaller than destination partition {partitionLength}({FormatSize(partitionLength)}). Replace?"))
                {
                    return ExitCode.Canceled;
                }
            }

            // Check to see if we're trying to put an HFS volume in a non-HFS partition.
            if (partition.Type != PartitionType.HierarchicalFileSystem)
            {
                HFSVolume hfs = new(source, position: 0);
                if ((hfs.BootBlockHeader.Signature == BootBlockHeader.ValidSignature
                    || hfs.BootBlockHeader.Signature == 0)
                    && hfs.MasterDirectoryBlock.Signature == MasterDirectoryBlock.ValidSignature)
                {
                    if (ConsoleService.QueryYesNo($"Make the target partition entry HFS?"))
                    {
                        partition.Type = PartitionType.HierarchicalFileSystem;
                        partition.Status = PartitionStatus.Valid | PartitionStatus.Allocated | PartitionStatus.InUse
                            | PartitionStatus.AllowsReading | PartitionStatus.AllowsWriting;
                        partition.Name = "MacOS";
                        partition.Save();
                    }
                }
            }

            StatusLog.WriteLine($"Replacing \"{partition.Name}\" partition...");
            target.Seek(startingPosition, SeekOrigin.Begin);
            target.CopyFrom(source, 0, source.Length);

            if (source.Length < partitionLength)
            {
                target.Erase(startingPosition + source.Length, partitionLength - source.Length);
            }
            StatusLog.WriteLine($"Partition replaced.");

            return ExitCode.Success;
        }

        public override string? Summary => "Replaces a partition with a specified file.";
        protected override string? GeneralHelp => Resources.HelpReplaceImage;
        protected override string? OptionDetails => Resources.HelpReplaceImageOptions;
    }
}
