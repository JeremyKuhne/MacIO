// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Services;

namespace xTask;

public abstract class TaskExecution
{
    private readonly ITaskRegistry _taskRegistry;

    protected TaskExecution(IArgumentProvider argumentProvider, ITaskRegistry taskRegistry, IServiceProvider? services)
    {
        ArgumentProvider = argumentProvider;
        _taskRegistry = taskRegistry;
        Services = services ?? new SimpleServiceProvider();
    }

    protected IArgumentProvider ArgumentProvider { get; }
    protected IServiceProvider Services { get; }

    protected abstract ITaskInteraction GetInteraction(ITask task);

    public ExitCode ExecuteTask()
    {
        ITask task = _taskRegistry[ArgumentProvider.Command];
        ITaskInteraction interaction = GetInteraction(task);

        ExitCode result = ExitCode.GeneralFailure;

        using (interaction as IDisposable)
        using (task as IDisposable)
        {
            if (ArgumentProvider.HelpRequested)
            {
                task.OutputUsage(interaction);
            }
            else
            {
                result = task.Execute(interaction);
            }
        }

        return result;
    }
}
