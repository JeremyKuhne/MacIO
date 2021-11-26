// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Used to have a single logger instance for all <see cref="LoggerType"/>s.
/// </summary>
public class SingleLogger : ILoggers
{
    private readonly ILogger _logger;
    public SingleLogger(ILogger logger) => _logger = logger;
    public ILogger this[LoggerType loggerType] => _logger;
}
