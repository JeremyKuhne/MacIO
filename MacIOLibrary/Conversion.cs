// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers.Binary;
using System.Text;

namespace MacIO;

public static class Conversion
{
    private static readonly DateTime s_1904 = new(1904, 1, 1, 0, 0, 0, DateTimeKind.Local);

    // https://archive.org/details/PDOS_2523028_Date_Format
    //
    //             Byte 1                          Byte 0
    //   7   6   5   4   3   2   1   0 | 7   6   5   4   3   2   1   0
    // +---------------------------------------------------------------+
    // |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
    // |           Year            |     Month     |        Day        |
    // |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
    // +---------------------------------------------------------------+
    //
    // +---------------------------------------------------------------+
    // |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
    // | 0   0   0 |       Hour        | 0   0 |        Minute         |
    // |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |
    // +---------------------------------------------------------------+
    //             Byte 1                          Byte 0
    private const ushort ProDOSYearMask   = 0b11111110_00000000;
    private const ushort ProDOSMonthMask  = 0b00000001_11100000;
    private const ushort ProDOSDayMask    = 0b00000000_00011111;
    private const ushort ProDOSHourMask   = 0b00011111_00000000;
    private const ushort ProDOSMinuteMask = 0b00000000_00111111;

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
    ///  Converts an ASCII byte source, terminating it at the first null, if found.
    /// </summary>
    public static string FromNullTerminatedAsciiString(ReadOnlySpan<byte> source)
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
    ///  Converts a Pascal style string.
    /// </summary>
    public static unsafe string FromPascalString(ReadOnlySpan<byte> source)
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

    /// <summary>
    ///  Converts an HFS date/time value to <see cref="DateTime"/>.
    /// </summary>
    public static DateTime FromHFSDate(uint hfsDateTime) => s_1904.AddSeconds(hfsDateTime);

    /// <summary>
    ///  Converts a ProDOS date/time combo to <see cref="DateTime"/>.
    /// </summary>
    public static DateTime FromProDOSDate(ushort date, ushort time)
    {
        int year = (date & ProDOSYearMask) >> 9;
        year += year < 40 ? 2000 : 1900;

        return new DateTime(
            year,
            (date & ProDOSMonthMask) >> 5,
            date & ProDOSDayMask,
            (time & ProDOSHourMask) >> 8,
            time & ProDOSMinuteMask,
            0,
            DateTimeKind.Local);
    }

    /// <summary>
    ///  Converts a <see cref="DateTime"/> to a ProDOS date/time.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown when years are not within the range of 1940-2039.
    /// </exception>
    public static (ushort date, ushort time) ToProDOSDate(DateTime dateTime)
    {
        if (dateTime.Year < 1940 || dateTime.Year > 2039)
            throw new ArgumentOutOfRangeException(nameof(dateTime), "ProDOS only supports years from 1940-2039");

        int year = dateTime.Year;
        year -= year > 1999 ? 2000 : 1900;
        int date = year << 9;
        date |= dateTime.Month << 5;
        date |= dateTime.Day;
        int time = dateTime.Hour << 8;
        time |= dateTime.Minute;

        return ((ushort)date, (ushort)time);
    }
}
