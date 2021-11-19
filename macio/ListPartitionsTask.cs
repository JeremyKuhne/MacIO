// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class ListPartitionsTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(stream);

            var map = image.PartitionMap;
            if (map.Count == 0)
            {
                StatusLog.WriteLine(WriteStyle.Error, "No partitions found.");
                return ExitCode.FileNotFound;
            }

            int? indexOption = Arguments.GetOption<int?>(StandardOptions.Index);

            Table table;
            if (indexOption.HasValue)
            {
                int index = indexOption.Value;
                if (index < 0 || index >= map.Count)
                {
                    StatusLog.WriteLine(WriteStyle.Error, $"Invalid index {index}. Entry count is {map.Count}.");
                    return ExitCode.InvalidArgument;
                }

                StatusLog.WriteLine(WriteStyle.Underline, $"Partition Entry {index}");

                var entry = map[index];
                table = Table.Create(1, 1);
                table.HasHeader = false;
                table.AddRow("Signature", $"{FormatSignature(entry.Signature)}");
                table.AddRow("Type", $"{entry.TypeString}");
                table.AddRow("Name", $"{entry.Name}");
                table.AddRow("Starting Block", $"{entry.PhysicalStart}");
                table.AddRow("Block Count", $"{entry.BlockCount}");
                table.AddRow("Size", $"{FormatSize((long)entry.BlockCount * 512)}");
                table.AddRow("Status", $"{entry.Status}");
                table.AddRow("Boot Address", $"{entry.BootAddress}");
                table.AddRow("Boot Size", $"{entry.BootSize} bytes");
                table.AddRow("Boot Checksum", $"{entry.BootChecksum}");
                table.AddRow("Boot Entry", $"{entry.BootEntry}");
                table.AddRow("Processor", $"{entry.ProcessorString}");

                ResultLog.Write(table);
                return ExitCode.Success;
            }

            table = Table.Create(1, 1, 1, 1, 1);
            table.HasHeader = true;

            table.AddRow("#", "Type", "Name", "Location", "Size");

            for (int i = 0; i < map.Count; i++)
            {
                var entry = map[i];
                table.AddRow(
                    $"{i}",
                    $"{entry.TypeString}",
                    $"{entry.Name}",
                    $"{entry.PhysicalStart}",
                    $"{entry.BlockCount}");
            }

            StatusLog.WriteLine("Partition entries (location/size in blocks)");
            StatusLog.WriteLine();
            ResultLog.Write(table);

            return ExitCode.Success;
        }

        public override string? Summary => "Lists the partitions in the image.";

        protected override string? GeneralHelp => Resources.HelpListPartitions;
        protected override string? OptionDetails => Resources.HelpListPartitionsOptions;
    }
}
