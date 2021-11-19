// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Settings;

namespace xTask;

public class ConsoleTaskExecution : TaskExecution
{
    /// <summary>
    ///  Execution handler for tasks running under the console.
    /// </summary>
    /// <param name="services">Override services, can be null. Used to get services before falling back on defaults.</param>
    public ConsoleTaskExecution(
        IArgumentProvider argumentProvider,
        ITaskRegistry taskRegistry,
        IServiceProvider? services = null)
        : base(argumentProvider, taskRegistry, services)
    {
    }

    protected override ITaskInteraction GetInteraction(ITask task)
        => ConsoleTaskInteraction.Create(ArgumentProvider, Services);
}
