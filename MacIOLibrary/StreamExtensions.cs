// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public static class StreamExtensions
{
    public unsafe static void ReadStruct<T>(this Stream stream, ref T data) where T : unmanaged
    {
        fixed (void* p = &data)
        {
            if (stream.Read(new Span<byte>(p, sizeof(T))) != sizeof(T))
            {
                throw new InvalidDataException($"Not enough data for a {typeof(T).Name}");
            }
        }
    }

    public unsafe static void WriteStruct<T>(this Stream stream, ref T data) where T : unmanaged
    {
        fixed (void* p = &data)
        {
            stream.Write(new Span<byte>(p, sizeof(T)));
        }
    }
}
