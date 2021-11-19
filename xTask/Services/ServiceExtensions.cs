// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System;

public static class ServiceExtensions
{
    public static T? GetService<T>(this IServiceProvider serviceProvider)
        where T : class
        => serviceProvider.GetService(typeof(T)) as T;

    public static T GetRequiredService<T>(this IServiceProvider serviceProvider)
        where T : class
        => serviceProvider.GetService(typeof(T)) as T
            ?? throw new KeyNotFoundException($"Could not find required service {typeof(T).Name}.");

    public static bool TryGetService<T>(this IServiceProvider serviceProvider, [NotNullWhen(true)] out T? service)
        where T : class
        => (service = serviceProvider.GetService<T>()) is not null;
}
