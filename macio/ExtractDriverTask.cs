// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class ExtractDriverTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream source = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(source);

            int index = Arguments.GetOption<int?>(StandardOptions.Index) ?? 0;

            var blockZero = image.BlockZero;
            var drivers = blockZero.DriverDescriptors;
            if (index >= drivers.Count)
            {
                StatusLog.WriteLine(WriteStyle.Error, $"Index {index} invalid. There are {drivers.Count} drivers.");
                return ExitCode.InvalidArgument;
            }

            var driver = drivers[index];
            var targets = Arguments.Targets;

            var partition = image.PartitionMap.First(e => e.PhysicalStart == driver.StartingBlock);

            string targetFile = targets.Count > 1
                ? targets[1]
                : $"{partition.Name}({partition.TypeString}-{partition.Processor}-{driver.OperatingSystemType})";

            using Stream target = FileService.CreateFileStream(targetFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            target.CopyFrom(source, driver.StartingBlock * blockZero.BlockSize, partition.BootSize);

            ResultLog.WriteLine($"Extracted to: {targetFile}");

            return ExitCode.Success;
        }

        public override string? Summary
            => "Extracts the specified driver code.";
    }
}
