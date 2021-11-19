// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Utility;

public static class Arrays
{
    /// <summary>
    ///  Compares two arrays for equivalency. All indicies must match.
    /// </summary>
    public static bool AreEquivalent<T>(T[] left, T[] right)
    {
        if (ReferenceEquals(left, right)) { return true; }

        if (left is null)
        {
            return right is null;
        }

        if (right is null)
        {
            return false;
        }

        return left.SequenceEqual(right);
    }

    /// <summary>
    ///  Simple helper to return an array's contents as a readable string
    /// </summary>
    public static string CreateString<T>(T[] array)
    {
        if (array == null) { return Resources.NullString; }
        if (array.Length == 0) { return Resources.EmptyString; }
        return string.Join(" ", array);
    }
}
