// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Logging;
using xTask.Services;
using xTask.Utility;

namespace xTask.Tasks;

/// <summary>
///  General help task.
/// </summary>
[Hidden]
public class HelpTask : ImplementedServiceProvider, ITask, ITaskDocumentation, ITaskExecutor
{
    private readonly ITaskRegistry _registry;
    private readonly string _generalHelp;

    public HelpTask(ITaskRegistry registry, string generalHelp)
    {
        _registry = registry;
        _generalHelp = generalHelp;
    }

    public string? Summary => null;

    public virtual ExitCode Execute(ITaskInteraction interaction)
    {
        interaction.Loggers[LoggerType.Result].WriteLine(WriteStyle.Fixed, _generalHelp);
        interaction.Loggers[LoggerType.Result].WriteLine();

        Table table = Table.Create(1, 1, 2);
        table.AddRow(Resources.OverviewColumn1, Resources.OverviewColumn2, Resources.OverviewColumn3);
        foreach (var entry in _registry.Tasks)
        {
            if (entry.Task.GetAttributes<HiddenAttribute>(inherit: true).Any())
            {
                continue;
            }

            string[] aliases = entry.Aliases.ToArray();
            if (aliases.Length == 0)
            {
                continue;
            }

            ITaskDocumentation? docs = entry.Task as ITaskDocumentation;

            table.AddRow(
                aliases[0],
                aliases.Length == 1 ? string.Empty : string.Join(", ", aliases.Skip(1)),
                docs?.Summary ?? string.Empty);
        }

        interaction.Loggers[LoggerType.Result].Write(table);

        return ExitCode.Success;
    }

    public void GetUsage(ITaskInteraction interaction)
    {
        Execute(interaction);
    }
}
