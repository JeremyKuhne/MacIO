// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Utility;

using System;
using System.Collections.Generic;

public static class Enums
{
    /// <summary>
    ///  Returns a list of set values in the given enum
    /// </summary>
    public static IEnumerable<T> GetSetValues<T>(T enumeration)
        where T : Enum
    {
        Enum castEnum = enumeration;

        List<T> setValues = new();
        foreach (object possibleValue in Enum.GetValues(typeof(T)))
        {
            if (castEnum.HasFlag((T)possibleValue))
            {
                setValues.Add((T)possibleValue);
            }
        }

        return setValues;
    }
}
