// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace xTask;

public static class ConsoleHelper
{
    /// <summary>
    ///  Access to the default Console
    /// </summary>
    public static IConsoleService Console => FlexServiceProvider.Services.GetRequiredService<IConsoleService>();

    /// <summary>
    ///  Does a locked write to the console in the specified foreground color, resetting when finished.
    /// </summary>
    public static void WriteLockedColor(this IConsoleService console, ConsoleColor color, string value)
    {
        try
        {
            console.ForegroundColor = color;
            console.Write(value);
        }
        finally
        {
            console.ResetColor();
        }
    }

    /// <summary>
    ///  Get a yes/no answer from the console.
    /// </summary>
    public static bool QueryYesNo(this IConsoleService console, string format, params object[] args)
        => console.QueryYesNo(string.Format(CultureInfo.CurrentUICulture, format, args));

    /// <summary>
    ///  Get a yes/no answer from the console.
    /// </summary>
    public static bool QueryYesNo(this IConsoleService console, string value)
    {
        string queryString = string.Format(CultureInfo.CurrentUICulture, Resources.YesNoQueryStringFormat, value);
        console.WriteLockedColor(console.ForegroundColor, queryString);
        console.Write("\n");
        string? answer = console.ReadLine()?.Trim();
        return string.Equals(answer, Resources.YesResponse, StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(answer, Resources.YesShortResponse, StringComparison.CurrentCultureIgnoreCase);
    }
}
