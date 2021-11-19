// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Tasks;

public abstract partial class TaskRegistry : ITaskRegistry
{
    private readonly List<TaskEntry> _tasks = new();
    private Func<ITask>? _defaultTask;

    public IEnumerable<ITaskEntry> Tasks => _tasks;

    protected void RegisterTaskInternal(Func<ITask> task, params string[] taskNames)
        => _tasks.Add(new TaskEntry(task, taskNames));

    protected void RegisterDefaultTaskInternal(Func<ITask> task) => _defaultTask = task;

    public ITask this[string taskName]
    {
        get
        {
            if (!string.IsNullOrEmpty(taskName))
            {
                foreach (var entry in _tasks)
                {
                    if (entry.Aliases.Contains(taskName))
                    {
                        return entry.Task;
                    }
                }
            }

            return _defaultTask?.Invoke() ?? new UnknownTask(this, Resources.HelpGeneral);
        }
    }
}
