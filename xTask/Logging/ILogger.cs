// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Interface for logging.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///  Writes the given string to the log.
    /// </summary>
    /// <param name="value">The string to write to the log.</param>
    void Write(string value);

    /// <summary>
    ///  Writes the given string in the given style to the log.
    /// </summary>
    /// <param name="style">The <see cref="WriteStyle"/> to write in.</param>
    /// <param name="value">The string to write to the log.</param>
    void Write(WriteStyle style, string value);

    /// <summary>
    ///  Writes the specfied table to the log.
    /// </summary>
    /// <param name="table">The <see cref="ITable"/> to write to the log.</param>
    void Write(ITable table);

    /// <summary>
    ///  Writes a blank line to the log.
    /// </summary>
    void WriteLine();

    /// <summary>
    ///  Writes the given line to the log.
    /// </summary>
    /// <param name="value">The line to log.</param>
    void WriteLine(string value);

    /// <summary>
    ///  Writes the given ling to the log in the given style.
    /// </summary>
    /// <param name="style">The <see cref="WriteStyle"/> to write in.</param>
    /// <param name="value">The line to write to the log.</param>
    void WriteLine(WriteStyle style, string value);
}
