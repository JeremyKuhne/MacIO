﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

public sealed partial class ConsoleTaskInteraction : TaskInteraction
{
    private readonly Lazy<ConsoleTaskLoggers> _loggers;

    private ConsoleTaskInteraction(IArgumentProvider arguments, IServiceProvider services)
        : base((arguments, services))
    {
        _loggers = new Lazy<ConsoleTaskLoggers>(() => new ConsoleTaskLoggers());
    }

    public static ITaskInteraction Create(IArgumentProvider arguments, IServiceProvider services)
        => new ConsoleTaskInteraction(arguments, services);

    public override object? GetService(Type serviceType)
    {
        object? result;
        if ((result = base.GetService(serviceType)) is null && serviceType == typeof(ILoggers))
        {
            result = _loggers.Value;
        }

        return result;
    }
}
