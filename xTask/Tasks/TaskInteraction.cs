// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Services;

namespace xTask;

/// <summary>
///  Base class for task interaction support.
/// </summary>
public abstract class TaskInteraction : ITaskInteraction
{
    private readonly IServiceProvider _services;

    protected TaskInteraction(IArgumentProvider arguments, IServiceProvider services)
    {
        Arguments = arguments;
        _services = services;
    }

    public IArgumentProvider Arguments { get; }
    public ILoggers Loggers => this.GetRequiredService<ILoggers>();

    protected abstract ILoggers GetDefaultLoggers();

    public object? GetService(Type serviceType)
    {
        object? service = _services?.GetService(serviceType) ?? FlexServiceProvider.Services.GetService(serviceType);
        if (service is not null)
        {
            return service;
        }

        return serviceType == typeof(ILoggers) ? GetDefaultLoggers() : null;
    }

    public virtual void Output(object value)
    {
        // Do nothing by default
    }
}
