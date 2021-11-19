// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Systems.Console;
using xTask.Utility;

namespace xTask.Logging;

public class ConsoleLogger : TextTableLogger
{
    private static readonly ConsoleLogger s_Instance = new();

    private readonly ConsoleColor _baseColor;
    private readonly List<ColorLookup> _colorTable;

    protected ConsoleLogger()
    {
        _baseColor = Console.ForegroundColor;
        _colorTable = new List<ColorLookup>
        {
            // Error state first
            new ColorLookup(WriteStyle.Critical | WriteStyle.Error, ConsoleColor.Red),
            new ColorLookup(WriteStyle.Italic | WriteStyle.Bold | WriteStyle.Important, ConsoleColor.Yellow)
        };
    }

    public static ILogger Instance => s_Instance;

    protected override void WriteInternal(WriteStyle style, string value)
    {
        ConsoleColor color = _baseColor;
        foreach (var lookup in _colorTable)
        {
            if ((lookup.WriteStyle & style) != 0)
            {
                color = lookup.ConsoleColor;
                break;
            }
        }

        if (style.HasFlag(WriteStyle.Underline))
        {
            WriteColorUnderlined(color, value);
        }
        else
        {
            WriteColor(color, value);
        }
    }

    private void WriteColorUnderlined(ConsoleColor color, string value)
        => WriteColor(color, Strings.Underline(value));

    protected virtual void WriteColor(ConsoleColor color, string value)
        => ConsoleHelper.Console.WriteLockedColor(color, value);

    protected override int TableWidth => Math.Max(80, Console.BufferWidth);

    private class ColorLookup : Tuple<WriteStyle, ConsoleColor>
    {
        public ColorLookup(WriteStyle writeStyle, ConsoleColor color)
            : base(writeStyle, color)
        {
        }

        public WriteStyle WriteStyle => Item1;
        public ConsoleColor ConsoleColor => Item2;
    }
}
