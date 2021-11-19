// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Tasks;
public abstract partial class TaskRegistry
{
    private class TaskEntry : ITaskEntry
    {
        private readonly HashSet<string> _aliases;
        private readonly Lazy<ITask> _task;

        public IEnumerable<string> Aliases => _aliases;

        public ITask Task => _task.Value;

        public TaskEntry(Func<ITask> task, params string[] taskNames)
        {
            _aliases = new HashSet<string>(taskNames, StringComparer.OrdinalIgnoreCase);
            _task = new Lazy<ITask>(task);
        }
    }
}
