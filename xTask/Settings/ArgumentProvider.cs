﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using xTask.Systems.File;
using xTask.Utility;

namespace xTask;

/// <summary>
///  Base implementation of IArgumentProvider functionality.
/// </summary>
public abstract class ArgumentProvider : IArgumentProvider
{
    // Guidance for argument syntax:
    // IEEE Std 1003.1 http://pubs.opengroup.org/onlinepubs/009695399/basedefs/xbd_chap12.html

    protected const string DefaultValue = "1";
    public const char MulitpleValueDelimiter = ';';
    private const char FileOptionDelimiter = '@';

    private readonly Dictionary<string, string> _options = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _targets = new();
    private string? _command;

    protected IFileService FileService { get; }

    public ArgumentProvider(IFileService fileService) => FileService = fileService;

    protected void AddTarget(string? target)
    {
        if (string.IsNullOrWhiteSpace(target)) return;

        switch (target.ToUpperInvariant())
        {
            case "HELP":
            case "?":
                // case "H":  "H" is short for help in tf.exe- avoiding this because H for history is more useful
                HelpRequested = true;
                return;
        }

        if (_command is null)
        {
            _command = target.Trim();
        }
        else
        {
            _targets.Add(target.Trim());
        }
    }

    protected void AddOrUpdateOption(string optionName, string? optionValue = null)
    {
        if (string.Equals(optionName, "target", StringComparison.OrdinalIgnoreCase)
            || string.Equals(optionName, "t", StringComparison.OrdinalIgnoreCase))
        {
            AddTarget(optionValue);
            return;
        }

        if (string.IsNullOrWhiteSpace(optionValue))
        {
            // Without a specifier, we'll store the value as String.Empty. "null" means "not set" to consumers.
            // Later we'll convert to the default value in any case that we ask for the value as a non-string.
            optionValue = string.Empty;
        }

        if (_options.ContainsKey(optionName))
        {
            // Append the new value
            _options[optionName] += MulitpleValueDelimiter + optionValue;
        }
        else
        {
            _options.Add(optionName, optionValue);
        }
    }

    protected IEnumerable<string> ReadFileLines(string fileName)
    {
        string path = FileService.GetFullPath(fileName.TrimStart(FileOptionDelimiter));

        if (!FileService.FileExists(path))
            throw new TaskArgumentException(Resources.ErrorFileNotFound, path);

        // (Somewhat akward, but cannot yield within a try with catch block)

        using Stream stream = FileService.CreateFileStream(path);
        IEnumerator<string> lineEnumerator = stream.ReadLines().GetEnumerator();

        bool moreLines;
        string line;

        do
        {
            try
            {
                moreLines = lineEnumerator.MoveNext();
                line = lineEnumerator.Current;
            }
            catch (Exception e)
            {
                throw new TaskArgumentException(e.Message, e);
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                // We don't want to risk recursive file references, handle the evil of lines like '@ @ @'
                line = line.Trim().TrimStart(FileOptionDelimiter);
                while (line.Length > 0 && char.IsWhiteSpace(line[0]))
                {
                    line = line.Trim().TrimStart(FileOptionDelimiter);
                }

                if (line.Length == 0) continue;

                yield return line;
            }

        } while (moreLines);
    }

    protected string PreprocessArgument(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value[0] != FileOptionDelimiter) return value;

        // Input file argument
        StringBuilder output = new();

        // File name, load lines
        foreach (string line in ReadFileLines(value))
        {
            output.Append(line);
            output.Append(MulitpleValueDelimiter);
        }

        if (output.Length == 0)
        {
            throw new TaskArgumentException(Resources.ErrorParameterFileInvalid, value);
        }

        // Trim off the last ';'
        output.Length--;
        return output.ToString();
    }

    public string? Target
    {
        get
        {
            if (_targets.Count == 0)
            {
                return null;
            }

            string? target = _targets[0];
            if (!string.IsNullOrEmpty(target) && target[0] == FileOptionDelimiter)
            {
                _targets.Clear();
                _targets.AddRange(ReadFileLines(target).ToArray());
                target = _targets.Count == 0 ? null : _targets[0];
            }

            return target;
        }
    }

    public IReadOnlyList<string> Targets
    {
        get
        {
            // Call the getter to initialize if necessary
            _ = Target;
            return _targets.ToArray();
        }
    }

    public string Command
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_command))
            {
                // General, unspecified help
                return "help";
            }

            return _command;
        }
    }

    public bool HelpRequested { get; private set; }

    public T? GetOption<T>(params string[] optionNames)
    {
        string? optionValue = null;

        foreach (string optionName in optionNames)
        {
            if (_options.TryGetValue(optionName, out optionValue))
            {
                // Preprocess the argument to allow handling of file based input (@).
                // We do this on demand to give as much opportunity for logging to
                // start up as possible.
                optionValue = PreprocessArgument(optionValue);
                _options[optionName] = optionValue;
                break;
            }
        }

        if (typeof(T) == typeof(string))
        {
            // No conversion needed, return as is
            return (T?)(object?)optionValue;
        }

        if (optionValue == string.Empty)
        {
            // Special case here- if we have no explicit value, use the default value to allow
            // converting to a logical "true".
            optionValue = DefaultValue;
        }

        return Types.ConvertType<T>(optionValue);
    }

    public IReadOnlyDictionary<string, string> Options => new Dictionary<string, string>(_options);
}
