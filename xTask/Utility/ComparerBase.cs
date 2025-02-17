﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Utility;

/// <summary>
///  Base class that eases setting up a class of related comparers (like StringComparer)
/// </summary>
public abstract class ComparerBase<T> : IComparer<T>, IEqualityComparer<T>
{
    private readonly IComparer<T> _comparer;
    private readonly IEqualityComparer<T> _equalityComparer;

    protected ComparerBase(IComparerImplementation implementation)
    {
        _comparer = implementation;
        _equalityComparer = implementation;
    }

    protected interface IComparerImplementation : IComparer<T>, IEqualityComparer<T>
    {
    }

    public int Compare(T? x, T? y) => _comparer.Compare(x, y);

    public bool Equals(T? x, T? y) => _equalityComparer.Equals(x, y);

    public int GetHashCode([DisallowNull] T obj) => _equalityComparer.GetHashCode(obj);
}
