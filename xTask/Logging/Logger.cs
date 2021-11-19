// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace xTask.Logging;

/// <summary>
///  Base <see cref="ILogger"/> implementation class.
/// </summary>
public abstract class Logger : ILogger
{
    private static string NewLine { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT ? "\r\n" : "\n";

    /// <inheritdoc/>
    public void Write(string value) => Write(WriteStyle.Current, value);

    /// <inheritdoc/>
    public void Write(WriteStyle style, string value)
        => WriteInternal(
            style,
            style.HasFlag(WriteStyle.Error)
                ? string.Format(CultureInfo.CurrentUICulture, Resources.ErrorFormatString, value)
                : value);

    /// <summary>
    ///  Write the given value in the given style to the log.
    /// </summary>
    /// <remarks>
    ///  This and <see cref="Write(ITable)"/> need overriden to provide logging functionality.
    /// </remarks>
    /// <param name="style">The <see cref="WriteStyle"/> to write in.</param>
    /// <param name="value">The string to write to the log.</param>
    protected abstract void WriteInternal(WriteStyle style, string value);

    /// <inheritdoc/>
    public void WriteLine() => Write(NewLine);

    /// <inheritdoc/>
    public void WriteLine(string value)
    {
        Write(value);
        WriteLine();
    }

    /// <inheritdoc/>
    public void WriteLine(WriteStyle style, string value)
    {
        Write(style, value);
        WriteLine();
    }

    /// <inheritdoc/>
    public abstract void Write(ITable table);
}
