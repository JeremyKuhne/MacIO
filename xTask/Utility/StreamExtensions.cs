// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Text;

namespace System.IO;

public static class StreamExtensions
{
    private static ReadOnlySpan<byte> EmptyBuffer => new byte[1024];

    /// <summary>
    ///  Attempts to get the length of the stream, handling any known cases where the stream doesn't support getting
    ///  length.
    /// </summary>
    /// <param name="stream">The stream to check length on.</param>
    /// <param name="length">The length of the stream.</param>
    /// <returns><see langword="true"/> if successful in getting the length.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static bool TryGetLength(this Stream stream, out long length)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        try
        {
            length = stream.Length;
            return true;
        }
        catch (Exception e) when (e.HResult == unchecked((int)0x80070001))
        {
            // When accessing a drive directly on Windows, trying to get the length will return ERROR_INVALID_FUNCTION.
            // https://github.com/JeremyKuhne/MacIO/issues/4
            length = 0;
            return false;
        }
    }

    /// <summary>
    ///  Enumerator for reading through lines on streams.
    /// </summary>
    /// <param name="stream">The stream to read lines from.</param>
    /// <param name="encoding">Optional encoding for the stream.</param>
    /// <returns>An enumerator that returns lines from the stream.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static IEnumerable<string> ReadLines(this Stream stream, Encoding? encoding = null)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        return ReadLinesIterator(stream, encoding ?? Encoding.Default);
    }

    private static IEnumerable<string> ReadLinesIterator(Stream stream, Encoding encoding)
    {
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
    /// <param name="stream">The stream to erase.</param>
    /// <param name="offset">The byte offset to begin erasing.</param>
    /// <param name="length">The number of bytes to erase.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  <paramref name="length"/> or <paramref name="offset"/> is null.
    /// </exception>
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
    ///  Copy the specified byte range from the source stream to the current position in the destination stream.
    /// </summary>
    /// <param name="destination">The stream to copy to.</param>
    /// <param name="source">The stream to copy from.</param>
    /// <param name="offset">The position in the <paramref name="source"/> to copy from.</param>
    /// <param name="length">The number of bytes to copy from the <paramref name="source"/>.</param>
    /// <param name="bufferSize">The optional buffer size for copying.</param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="source"/> or <paramref name="destination"/> is <see langref="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  <paramref name="offset"/> or <paramref name="length"/> is less than zero or <paramref name="bufferSize"/>
    ///  is less than one.
    /// </exception>
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

        // Do not check stream length in this method as it will throw when directly accessing
        // drives on Windows.

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
