// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Systems.File;
using xTask.Systems.File.Concrete;

namespace TestProject;

public class TestFileCleaner : FileCleaner
{
    public TestFileCleaner()
        : base("MacIOTests", new FileService())
    {
    }
}
