// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Logging;

namespace xTask;

public sealed partial class ConsoleTaskInteraction
{
    private sealed class ConsoleTaskLoggers : Loggers
    {
        public ConsoleTaskLoggers()
        {
            RegisterLogger(LoggerType.Result, ConsoleLogger.Instance);
            RegisterLogger(LoggerType.Status, ConsoleLogger.Instance);
        }
    }
}
