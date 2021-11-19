// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

internal static class TaskExtensions
{
    /// <summary>
    ///  Gets the option default for the given option.
    /// </summary>
    internal static T? GetOptionDefault<T>(this ITask task, string option)
    {
        return task.TryGetService(out ITaskOptionDefaults? optionDefaults)
            ? optionDefaults.GetOptionDefault<T>(option)
            : default;
    }

    /// <summary>
    ///  Outputs usage if any help is provided.
    /// </summary>
    public static void OutputUsage(this ITask task, ITaskInteraction interaction)
    {
        if (task.TryGetService(out ITaskDocumentation? documentation))
        {
            documentation.GetUsage(interaction);
        }
        else
        {
            interaction.Loggers[LoggerType.Result].WriteLine(WriteStyle.Fixed, Resources.HelpNone);
        }
    }

    /// <summary>
    ///  Executes the given task.
    /// </summary>
    public static ExitCode Execute(this ITask task, ITaskInteraction interaction)
    {
        return task.TryGetService(out ITaskExecutor? executor)
            ? executor.Execute(interaction)
            : ExitCode.Success;
    }
}
