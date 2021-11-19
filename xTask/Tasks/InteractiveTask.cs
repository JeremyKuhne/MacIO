// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Settings;
using xTask.Systems.Console;
using xTask.Systems.File;
using xTask.Utility;

namespace xTask.Tasks;

/// <summary>
///  Puts task into an interactive mode where multiple commands can be entered.
/// </summary>
public class InteractiveTask : Task
{
    private readonly string _prompt;
    private readonly IConsoleService _consoleService;
    private readonly ITaskRegistry _registry;
    private static readonly string[] s_QuitCommands = { "quit", "q", "exit" };

    public InteractiveTask(string prompt, ITaskRegistry registry, IConsoleService? consoleService = null)
    {
        _prompt = prompt;
        _registry = registry;
        _consoleService = consoleService ?? ConsoleHelper.Console;
    }

    protected override ExitCode ExecuteInternal()
    {
        string? input;
        do
        {
            _consoleService.Write(_prompt);
            input = Environment.ExpandEnvironmentVariables(_consoleService.ReadLine().Trim());
            if (s_QuitCommands.Contains(input, StringComparer.OrdinalIgnoreCase))
            {
                break;
            }

            CommandLineParser parser = new(this.GetRequiredService<IFileService>());
            parser.Parse(Strings.SplitCommandLine(input).ToArray());
            ConsoleTaskExecution execution = new(parser, _registry);
            execution.ExecuteTask();
        } while (true);

        return ExitCode.Success;
    }

    public override string Summary => Resources.InteractiveTaskSummary;
}
