// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using xTask.Systems.Console.Concrete;
using xTask.Systems.File.Concrete;

namespace xTask;

/// <summary>
///  Flexible services provider.
/// </summary>
public static class FlexServiceProvider
{
    private static readonly SimpleServiceProvider s_concreteServices;

    static FlexServiceProvider()
    {
        s_concreteServices = new SimpleServiceProvider();
        s_concreteServices.AddService<IFileService>(new FileService());
        s_concreteServices.AddService<IConsoleService>(new ConcreteConsoleService());
    }

    public static IServiceProvider Services => s_concreteServices;
}
