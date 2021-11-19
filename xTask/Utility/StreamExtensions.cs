// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Text;

namespace System.IO;

public static class StreamExtensions
{
    private static ReadOnlySpan<byte> EmptyBuffer => new byte[1024];

    /// <summary>
    ///  Enumerator for reading through lines on streams.
    /// </summary>
    public static IEnumerable<string> ReadLines(this Stream stream, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;
        using StreamReader reader = new(stream, encoding, leaveOpen: true);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return line;
        }
    }

    /// <summary>
    ///  Erase the specified section of the stream.
    /// </summary>
    public static void Erase(this Stream stream, long offset, long length)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if (stream.Position != offset)
        {
            stream.Position = offset;
        }

        var buffer = EmptyBuffer;
        while (length > 0)
        {
            if (length < buffer.Length)
            {
                buffer = buffer[..(int)length];
            }

            length -= buffer.Length;
            stream.Write(buffer);
        }
    }

    /// <summary>
    ///  Copy the specified byte range from the source stream.
    /// </summary>
    public static void CopyFrom(this Stream destination, Stream source, long offset, long length, int bufferSize = 4096)
    {
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
        if (bufferSize < 1)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));

        if (source.Position != offset)
        {
            source.Position = offset;
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        int bytesRead;
        int read = (int)Math.Min(length, buffer.Length);

        try
        {
            while (read > 0 && (bytesRead = source.Read(buffer, 0, read)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
                length -= bytesRead;
                read = (int)Math.Min(length, buffer.Length);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
