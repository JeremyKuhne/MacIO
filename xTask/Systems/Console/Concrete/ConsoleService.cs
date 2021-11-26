// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Systems.Console.Concrete;

using System;

/// <summary>
///  Simple thunk to <see cref="Console"/>.
/// </summary>
public class ConsoleService : IConsoleService
{
    private static readonly object s_SyncLock = new();

    public void Write(string value)
    {
        // Need to lock to prevent interleaved output from multiple threads (and messed up colors)
        lock (s_SyncLock)
        {
            Console.Write(value);
        }
    }

    public string ReadLine() => Console.ReadLine() ?? string.Empty;

    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }

    public void ResetColor() => Console.ResetColor();
}
