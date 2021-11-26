// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

/// <summary>
///  Standard command line argument parser.
/// </summary>
public class CommandLineParser : ArgumentProvider
{
    public CommandLineParser(IFileService fileService)
        : base(fileService)
    {
    }

    public void Parse(params string[] arguments)
    {
        if (arguments is null || arguments.Length == 0)
        {
            return;
        }

        for (int i = 0; i < arguments.Length; i++)
        {
            string argument = arguments[i];
            if (string.IsNullOrWhiteSpace(argument))
            {
                continue;
            }

            argument = argument.Trim();

            switch (argument[0])
            {
                case '/':
                case '-':
                    argument = argument[1..].Trim();
                    int colonIndex = argument.IndexOf(':');
                    if (colonIndex == -1)
                    {
                        AddOrUpdateOption(argument);
                    }
                    else
                    {
                        AddOrUpdateOption(argument[..colonIndex], argument[(colonIndex + 1)..]);
                    }
                    break;
                default:
                    AddTarget(argument);
                    break;
            }
        }
    }
}
