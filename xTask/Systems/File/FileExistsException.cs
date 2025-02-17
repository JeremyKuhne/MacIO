﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace xTask.Systems.File;

/// <summary>
///  Thrown when a file or directory exists and invalidates the current operation.
/// </summary>
[Serializable]
public class FileExistsException : IOException
{
    public FileExistsException(string format, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, format, args))
    {
    }
}
