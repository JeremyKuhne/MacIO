// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class HFSInfoTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(stream);

            long location = 0;

            if (image.BlockZero.Signature == BlockZero.ValidSignature)
            {
                // Normal drive image.

                int? indexOption = Arguments.GetOption<int?>(StandardOptions.Index);
                var map = image.PartitionMap;

                if (indexOption.HasValue)
                {
                    int index = indexOption.Value;
                    if (index < 0 || index >= map.Count)
                    {
                        StatusLog.WriteLine(WriteStyle.Error, $"Invalid index {index}. Entry count is {map.Count}.");
                        return ExitCode.InvalidArgument;
                    }

                    var entry = map[index];
                    if (entry.Type != PartitionType.HierarchicalFileSystem)
                    {
                        StatusLog.WriteLine(WriteStyle.Error, $"Index {index} is not HFS. Parition type is \"{entry.TypeString}\".");
                        return ExitCode.InvalidData;
                    }

                    location = entry.PhysicalStart * image.BlockZero.BlockSize;
                }
                else
                {
                    var entry = map.FirstOrDefault(e => e.Type == PartitionType.HierarchicalFileSystem);
                    if (entry is null)
                    {
                        StatusLog.WriteLine(WriteStyle.Error, $"No HFS partitions found.");
                        return ExitCode.InvalidData;
                    }

                    location = entry.PhysicalStart * image.BlockZero.BlockSize;
                }
            }

            HFSVolume hfs = new(stream, position: location, readOnly: true);

            Table table = Table.Create(1, 1);
            table.HasHeader = false;

            if (hfs.BootBlockHeader.Signature == 0)
            {
                table.AddRow("Boot Block", "<Uninitialized>");
            }
            else
            {
                table.AddRow("Boot Block Signature", $"{FormatSignature(hfs.BootBlockHeader.Signature)}");
                table.AddRow("System File", $"{hfs.BootBlockHeader.SystemFileName}");
                table.AddRow("Shell File", $"{hfs.BootBlockHeader.ShellFileName}");
                table.AddRow("First Debugger", $"{hfs.BootBlockHeader.FirstDebugger}");
                table.AddRow("Second Debugger", $"{hfs.BootBlockHeader.SecondDebugger}");
                table.AddRow("Startup Screen", $"{hfs.BootBlockHeader.StartupScreen}");
                table.AddRow("Startup Program", $"{hfs.BootBlockHeader.StartupProgram}");
                table.AddRow("Scrap File", $"{hfs.BootBlockHeader.ScrapName}");
            }

            table.AddRow("Volume Name", $"{hfs.MasterDirectoryBlock.VolumeName}");
            table.AddRow("Signature", $"{FormatSignature(hfs.MasterDirectoryBlock.Signature)}");
            table.AddRow("Creation Time", $"{hfs.MasterDirectoryBlock.VolumeCreationTime}");
            table.AddRow("Last Modification", $"{hfs.MasterDirectoryBlock.LastModificationTime}");
            table.AddRow("Volume Attributes", $"{hfs.MasterDirectoryBlock.VolumeAttributes}");
            table.AddRow("Allocation Block Size", $"{FormatSize(hfs.MasterDirectoryBlock.AllocationBlockSize)}");
            table.AddRow("Number of Blocks", $"{hfs.MasterDirectoryBlock.NumberOfAllocationBlocks}");
            table.AddRow("Unused Blocks", $"{hfs.MasterDirectoryBlock.UnusedAllocationBlocks}");
            table.AddRow("Number of Files", $"{hfs.MasterDirectoryBlock.NumberOfFiles}");
            table.AddRow("Number of Directories", $"{hfs.MasterDirectoryBlock.NumberOfDirectories}");

            ResultLog.Write(table);

            return ExitCode.Success;
        }

        public override string? Summary => "Lists information for HFS volumes.";
        protected override string? GeneralHelp => Resources.HelpHFSInfo;
        protected override string? OptionDetails => Resources.HelpHFSInfoOptions;
    }
}
