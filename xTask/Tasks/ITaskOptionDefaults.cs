// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

public interface ITaskOptionDefaults
{
    /// <summary>
    ///  Get defaults for the given option, if any.
    /// </summary>
    T GetOptionDefault<T>(string option);
}
