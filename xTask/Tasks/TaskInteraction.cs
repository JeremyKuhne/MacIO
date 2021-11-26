// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Base class for task interaction support.
/// </summary>
public abstract class TaskInteraction : ITaskInteraction
{
    private readonly IServiceProvider _services;

    protected TaskInteraction((IArgumentProvider arguments, IServiceProvider services) parameters)
    {
        Arguments = parameters.arguments;
        _services = parameters.services;
    }

    public IArgumentProvider Arguments { get; }
    public ILoggers Loggers => this.GetRequiredService<ILoggers>();

    public virtual object? GetService(Type serviceType)
        => _services?.GetService(serviceType) ?? FlexServiceProvider.Services.GetService(serviceType);

    public virtual void Output(object value)
    {
        // Do nothing by default
    }
}
