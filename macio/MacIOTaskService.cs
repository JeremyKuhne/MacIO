// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;
using xTask.Tasks;

namespace MacIO;

public class MacIOTaskService : TaskService
{
    private MacIOTaskService()
        : base("A utility for manipulating and creating Macintosh 68K drive images.")
    {
    }

    public static ITaskService Create()
    {
        MacIOTaskService taskService = new();
        taskService.Initialize();
        return taskService;
    }

    public override void Initialize()
    {
        SimpleTaskRegistry registry = GetTaskRegistry();

        registry.RegisterTask(() => new ImageInfoTask(), "info");
        registry.RegisterTask(() => new BlockZeroTask(), "blockzero", "bz");
        registry.RegisterTask(() => new ListPartitionsTask(), "listpartitions", "lp");
        registry.RegisterTask(() => new DeletePartitionTask(), "deletepartition", "dp");
        registry.RegisterTask(() => new ChecksumTask(), "checksum", "hash", "cs");
        registry.RegisterTask(() => new ExtractDriverTask(), "extractdriver", "ed");
        registry.RegisterTask(() => new ExtractPartitionTask(), "extractpartition", "ep");
        registry.RegisterTask(() => new ReplacePartitionTask(), "replacepartition", "rp");
        registry.RegisterTask(() => new HFSInfoTask(), "hfsinfo", "hfs", "hi");
        registry.RegisterTask(() => new CreateImageTask(), "createimage", "ci");

        base.Initialize();
    }
}
