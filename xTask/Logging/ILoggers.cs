﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

public interface ILoggers
{
    ILogger this[LoggerType loggerType] { get; }
}
