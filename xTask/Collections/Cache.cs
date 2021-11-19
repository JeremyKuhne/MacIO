// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Collections;

using System;
using System.Threading;

/// <summary>
///  Light weight multithreaded fixed size cache class.
/// </summary>
public class Cache<T> : IDisposable where T : class, new()
{
    private readonly T?[] _itemsCache;

    /// <summary>
    ///  Create a cache with space for the specified number of items.
    /// </summary>
    public Cache(int cacheSpace)
    {
        if (cacheSpace < 1) cacheSpace = Environment.ProcessorCount * 4;
        _itemsCache = new T?[cacheSpace];
    }

    /// <summary>
    ///  Get an item from the cache or create one if none are available.
    /// </summary>
    public virtual T Acquire()
    {
        T? item;

        for (int i = 0; i < _itemsCache.Length; i++)
        {
            item = Interlocked.Exchange(ref _itemsCache[i], null);
            if (item is not null) return item;
        }

        return new T();
    }

    /// <summary>
    ///  Release an item back to the cache, disposing if no room is available.
    /// </summary>
    public virtual void Release(T item)
    {
        T? removedItem = item;
        for (int i = 0; i < _itemsCache.Length; i++)
        {
            removedItem = Interlocked.Exchange(ref _itemsCache[i], removedItem);
            if (removedItem is null) return;
        }

        (removedItem as IDisposable)?.Dispose();
    }

    /// <summary>
    ///  Disposes the <see cref="Cache{T}"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Disposes the <see cref="Cache{T}"/>.
    /// </summary>
    /// <param name="disposing">True if coming from dispose. Set to false if invoking from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int i = 0; i < _itemsCache.Length; i++)
            {
                (_itemsCache[i] as IDisposable)?.Dispose();
                _itemsCache[i] = null;
            }
        }
    }
}
