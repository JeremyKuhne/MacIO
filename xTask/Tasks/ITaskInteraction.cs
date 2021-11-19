// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Logging;

namespace xTask;

/// <summary>
///  Input and output interfaces and services for a task
/// </summary>
public interface ITaskInteraction : IServiceProvider
{
    IArgumentProvider Arguments { get; }
    ILoggers Loggers { get; }
    void Output(object value);
}
