// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Task registry.
/// </summary>
public interface ITaskRegistry
{
    /// <summary>
    ///  Returns the task with the given task name or alias, if any.
    /// </summary>
    ITask this[string taskName] { get; }

    /// <summary>
    ///  Returns a list of all available tasks.
    /// </summary>
    IEnumerable<ITaskEntry> Tasks { get; }
}
