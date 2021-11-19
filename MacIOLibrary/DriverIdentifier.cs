// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using static MacIO.Conversion;

namespace MacIO;

public class DriverIdentifier
{
    private DriverData _data;

    internal DriverData Data => _data;

    internal DriverIdentifier(DriverData data) => _data = data;

    public DriverIdentifier() {}

    /// <summary>
    ///  Driver starting block.
    /// </summary>
    public uint StartingBlock
    {
        get => BigEndian(_data.ddBlock);
        set => _data.ddBlock = BigEndian(value);
    }

    /// <summary>
    ///  Size of the driver in 512 byte blocks.
    /// </summary>
    public ushort SizeInBlocks
    {
        get => BigEndian(_data.ddSize);
        set => _data.ddSize = BigEndian(value);
    }

    /// <summary>
    ///  OS Type.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   MacOS has a value of '1'.
    ///  </para>
    ///  <para>
    ///   Start Manager "SetOSDefault()" is used to pick which type is desired.
    ///  </para>
    /// </remarks>
    public ushort OperatingSystemType
    {
        get => BigEndian(_data.ddType);
        set => _data.ddType = BigEndian(value);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverData
    {
        public uint ddBlock;            // driver's starting block
        public ushort ddSize;           // size of the driver, in 512-byte blocks
        public ushort ddType;           // operating system type (MacOS = 1)
    }
}
