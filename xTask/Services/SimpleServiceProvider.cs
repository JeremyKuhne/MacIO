// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask;

public class SimpleServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _services = new();

    public void AddService<T>(T service) where T : class
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        _services.Add(typeof(T), service);
    }

    public object? GetService(Type serviceType)
    {
        _services.TryGetValue(serviceType, out object? value);
        return value;
    }
}
