// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace xTask.Utility;

public static class Hash
{
    private static readonly MD5 s_MD5 = MD5.Create();

    /// <summary>
    ///  Gets a Base64 encoded hash for the given <paramref name="stream"/>.
    /// </summary>
    public static string GetHash(Stream stream)
        => Convert.ToBase64String(s_MD5.ComputeHash(stream));

    /// <summary>
    ///  Gets a Base64 encoded hash for the given <paramref name="data"/>.
    /// </summary>
    public static string GetHash(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[16];
        if (!s_MD5.TryComputeHash(data, hash, out _))
        {
            throw new InvalidOperationException();
        }
        return Convert.ToBase64String(hash);
    }
}
