// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask;

namespace MacIO;

class Program
{
    static int Main(string[] args)
    {
        ExitCode result = ExitCode.GeneralFailure;

        CommandLineParser parser = new(FlexServiceProvider.Services.GetRequiredService<IFileService>());
        parser.Parse(args);

        using (ITaskService taskService = MacIOTaskService.Create())
        {
            ConsoleTaskExecution execution = new(parser, taskService.TaskRegistry);
            result = execution.ExecuteTask();
        }

        return (int)result;
    }
}
