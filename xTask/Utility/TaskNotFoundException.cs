﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using xTask.Tasks;

namespace xTask.Utility;

/// <summary>
///  Used to centrally handle missing items.
/// </summary>
public class TaskNotFoundException : TaskException
{
    public TaskNotFoundException(string item)
        : base(string.Format(CultureInfo.CurrentUICulture, Resources.CouldNotFindGeneral, item))
    {
    }

    public TaskNotFoundException(string item, string detailMessage)
        : base(string.Format(CultureInfo.CurrentUICulture, Resources.CouldNotFindGeneral, item, detailMessage))
    {
    }

    public override ExitCode ExitCode => ExitCode.PathNotFound;
}
