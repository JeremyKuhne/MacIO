// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Logging;
using xTask.Services;
using xTask.Settings;

namespace xTask.Tasks;

public abstract class Task : ImplementedServiceProvider, ITask, ITaskExecutor, ITaskDocumentation
{
    private ITaskInteraction? _interaction;

    public ExitCode Execute(ITaskInteraction interaction)
    {
        _interaction = interaction;
        return ExecuteInternal();
    }

    protected abstract ExitCode ExecuteInternal();

    protected IArgumentProvider Arguments => _interaction!.Arguments;
    protected ILoggers Loggers => _interaction!.Loggers;

    protected ILogger StatusLog => Loggers[LoggerType.Status];
    protected ILogger ResultLog => Loggers[LoggerType.Result];

    protected void Output(object value) => _interaction?.Output(value);

    public override object? GetService(Type serviceType)
    {
        // Try to get services from:
        // 1. Base (implementation)
        // 2. TaskInteraction
        // 3. DefaultServices

        return base.GetService(serviceType)
            ?? _interaction?.GetService(serviceType)
            ?? FlexServiceProvider.Services.GetService(serviceType);
    }

    public void GetUsage(ITaskInteraction interaction)
    {
        // Guidance for argument syntax:
        // IEEE Std 1003.1 http://pubs.opengroup.org/onlinepubs/009695399/basedefs/xbd_chap12.html

        ILogger logger = interaction.Loggers[LoggerType.Result];
        if (string.IsNullOrEmpty(GeneralHelp))
        {
            logger.Write(Resources.HelpNone);
            return;
        }

        logger.WriteLine(WriteStyle.Fixed, GeneralHelp);

        string? optionDetails = OptionDetails;
        if (string.IsNullOrEmpty(optionDetails))
        {
            return;
        }

        logger.WriteLine();
        logger.WriteLine(WriteStyle.Fixed | WriteStyle.Underline, Resources.HelpOptionDetailHeader);
        logger.WriteLine();
        logger.WriteLine(WriteStyle.Fixed, optionDetails);
    }


    protected virtual string? GeneralHelp => null;
    protected virtual string? OptionDetails => null;

    public virtual string? Summary => null;
}
