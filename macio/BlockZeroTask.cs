// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO
{
    public class BlockZeroTask : FileTask
    {
        protected override ExitCode ExecuteFileTask()
        {
            using Stream stream = FileService.CreateFileStream(GetFullTargetPath());
            DriveImage image = new(stream);

            var blockZero = image.BlockZero;

            Table table = Table.Create(1, 1);
            table.HasHeader = false;

            table.AddRow("Signature", $"{FormatSignature(blockZero.Signature)}");
            table.AddRow("Block Size", $"{blockZero.BlockSize}");
            table.AddRow("Block Count", $"{blockZero.NumberOfBlocks}");
            table.AddRow("Image Size", $"{FormatSize(blockZero.NumberOfBlocks * blockZero.BlockSize)}");

            var drivers = blockZero.DriverDescriptors;
            for(int i = 0; i < drivers.Count; i++)
            {
                var driver = drivers[i];
                table.AddRow($"Driver {i} Starting Block", $"{driver.StartingBlock}");
                table.AddRow($"Driver {i} Block Count", $"{driver.SizeInBlocks}");
                table.AddRow($"Driver {i} Disk Size", $"{FormatSize(driver.SizeInBlocks * blockZero.BlockSize)}");
                table.AddRow($"Driver {i} OS Type", $"{driver.OperatingSystemType}");
            }

            ResultLog.Write(table);
            return ExitCode.Success;
        }

        protected override string? GeneralHelp
            => "Lists the zero block information.";
    }
}
