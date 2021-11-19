// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

/// <summary>
///  Stream wrapper that only allows reading.
/// </summary>
public class ReadOnlyStream : Stream
{
    private readonly Stream _stream;

    public ReadOnlyStream(Stream stream)
    {
        _stream = stream;
    }

    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public override void Flush() => _stream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => _stream.Read(buffer, offset, count);

    public override int Read(Span<byte> buffer)
        => _stream.Read(buffer);

    public override long Seek(long offset, SeekOrigin origin)
        => _stream.Seek(offset, origin);

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();
}
