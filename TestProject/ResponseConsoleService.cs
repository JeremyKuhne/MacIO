// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

/// <summary>
///  Simple test <see cref="IConsoleService"/> that returns "yes" to line reads.
/// </summary>
public class ResponseConsoleService : IConsoleService
{
    /// <summary>
    ///  The next line that will be returned from <see cref="ReadLine"/>.
    /// </summary>
    public string NextReadLine { get; set; } = string.Empty;

    public ConsoleColor ForegroundColor { get; set; }
    public string ReadLine() => NextReadLine;
    public void ResetColor() { }
    public void Write(string value) { }
}
