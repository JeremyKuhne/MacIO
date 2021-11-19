// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers.Binary;
using System.Text;

namespace MacIO;

public static class Conversion
{

    private static readonly DateTime s_1904 = new(1904, 1, 1, 0, 0, 0, DateTimeKind.Local);

    /// <summary>
    ///  Flips the endian-ness for the given <paramref name="value"/> if the current
    ///  platform is little-endian.
    /// </summary>
    public static ushort BigEndian(ushort value) => BitConverter.IsLittleEndian
        ? BinaryPrimitives.ReverseEndianness(value)
        : value;

    /// <summary>
    ///  Flips the endian-ness for the given <paramref name="value"/> if the current
    ///  platform is little-endian.
    /// </summary>
    public  static uint BigEndian(uint value) => BitConverter.IsLittleEndian
        ? BinaryPrimitives.ReverseEndianness(value)
        : value;

    /// <summary>
    ///  Returns a string from the given ASCII byte source, terminating it at the first null.
    /// </summary>
    public static string GetNullTerminatedAsciiString(ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        int length = source.IndexOf((byte)0x00);
        if (length == 0)
        {
            return string.Empty;
        }

        return Encoding.ASCII.GetString(source[..(length == -1 ? source.Length : length)]);
    }

    /// <summary>
    ///  Returns a Pascal string from the given ASCII byte source.
    /// </summary>
    public static unsafe string GetPascalString(ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        int length = source[0];
        if (length == 0)
        {
            return string.Empty;
        }

        return Encoding.ASCII.GetString(source.Slice(1, length));
    }

    // Time in non-leap seconds from 1904 (max is 02/06/2040)
    // It is normally in local time for HFS
    // An HFS Plus-timestamp is the number of seconds since midnight, January 1, 1904, GMT.

    public static DateTime GetHFSDate(uint hfsTime) => s_1904.AddSeconds(hfsTime);
}
