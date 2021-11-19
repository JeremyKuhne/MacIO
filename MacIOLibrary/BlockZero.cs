// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static MacIO.Conversion;

namespace MacIO;

/// <summary>
///  The initial disk block that that identifies the disk size and the available device drivers.
/// </summary>
/// <remarks>
///  <para>
///   This record always resides in physical block 0 (the first 512 bytes).
///  </para>
///  <para>
///   This record is described in "Inside Macintosh: Devices" in
///   <see href="https://developer.apple.com/library/archive/documentation/mac/Devices/Devices-121.html#MARKER-9-35">
///   "Chapter 3: SCSI Manager".</see>
///  </para>
/// </remarks>
public partial class BlockZero : Record
{
    private const int MinimumSize = 512;

    private readonly byte[] _data = new byte[MinimumSize];
    private ref Block0 BlockData => ref Unsafe.As<byte, Block0>(ref Unsafe.AsRef(_data[0]));

    public BlockZero(Stream stream, bool readOnly = true)
        : base(stream, readOnly)
    {
        if (Stream.CanWrite && stream.Length < MinimumSize)
        {
            stream.SetLength(MinimumSize);
        }

        Stream.Seek(0, SeekOrigin.Begin);
        stream.Read(_data);
    }

    /// <summary>
    ///  "ER"
    /// </summary>
    public const ushort ValidSignature = 0x4552;

    /// <summary>
    ///  Record signature.
    /// </summary>
    /// <remarks>
    ///  Presence of this signature is used to indicate that the drive is formatted.
    /// </remarks>
    public ushort Signature
    {
        get => BigEndian(BlockData.sbSig);
        set => BlockData.sbSig = BigEndian(value);
    }

    /// <summary>
    ///  Block size of the device. (Typically 0x0200, or 512 bytes)
    /// </summary>
    public ushort BlockSize
    {
        get => BigEndian(BlockData.sbBlkSize);
        set
        {
            if (value < 512)
            {
                // A block size of less than 512 would mess up the partition table which requires
                // 512 bytes
                throw new ArgumentOutOfRangeException(nameof(value), "Block size must be at least 512 bytes.");
            }
            BlockData.sbBlkSize = BigEndian(value);
        }
    }

    /// <summary>
    ///  Number of blocks on the device.
    /// </summary>
    public uint NumberOfBlocks
    {
        get => BigEndian(BlockData.sbBlkCount);
        set => BlockData.sbBlkCount = BigEndian(value);
    }

    /// <summary>
    ///  Number of driver descriptor entries in the partition map.
    /// </summary>
    private ushort Entries
    {
        get => BigEndian(BlockData.sbDrvrCount);
        set => BlockData.sbDrvrCount = BigEndian(value);
    }

    /// <summary>
    ///  Reserved.
    /// </summary>
    public ushort DevType
    {
        get => BigEndian(BlockData.sbDevType);
        set => BlockData.sbDevType = BigEndian(value);
    }

    /// <summary>
    ///  Reserved.
    /// </summary>
    public ushort DevId
    {
        get => BigEndian(BlockData.sbDevId);
        set => BlockData.sbDevId = BigEndian(value);
    }

    /// <summary>
    ///  Reserved.
    /// </summary>
    public uint Data
    {
        get => BigEndian(BlockData.sbData);
        set => BlockData.sbData = BigEndian(value);
    }

    public unsafe IList<DriverIdentifier> DriverDescriptors
    {
        get
        {
            List<DriverIdentifier> drivers = new(Entries);
            fixed (void* d = _data)
            {
                ReadOnlySpan<DriverIdentifier.DriverData> data = new((byte*)d + sizeof(Block0), Entries);
                for (int i = 0; i < Entries; i++)
                {
                    drivers.Add(new(data[i]));
                }
            }
            return drivers;
        }
        set
        {
            ushort count = (ushort)value.Count;
            if (count * sizeof(DriverIdentifier.DriverData) + sizeof(Block0) > _data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Entries = (ushort)value.Count;

            fixed (void* d = _data)
            {
                Span<DriverIdentifier.DriverData> data = new((byte*)d + sizeof(Block0), Entries);
                for (int i = 0; i < Entries; i++)
                {
                    data[i] = value[i].Data;
                }
            }
        }
    }

    public override void Save()
    {
        Stream.Seek(0, SeekOrigin.Begin);
        Stream.Write(_data);
        Stream.Flush();
    }

    // The data is in big endian format
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct Block0
    {
        public ushort sbSig;            // device signature
        public ushort sbBlkSize;        // block size of the device
        public uint sbBlkCount;         // number of blocks on the device
        public ushort sbDevType;        // reserved
        public ushort sbDevId;          // reserved
        public uint sbData;             // reserved
        public ushort sbDrvrCount;      // number of driver descriptor entries
        //public uint ddBlock;          // first driver's starting block
        //public ushort ddSize;         // size of the driver, in 512-byte blocks
        //public ushort ddType;         // operating system type (MacOS = 1)

        // Additional driver info can follow as repeating ddBlock/ddSize/ddType from here.

        // Sample Data
        //
        // Signature:       0x4552
        // Block size:      0x0200 (512)
        // Block count:     0x00019000 (With above ~50MB)
        // Reserved:        0x0000000000000000
        // Driver count:    0x0001
        // Starting block:  0x00000040 (At physical address 0x8000)
        // Driver size:     0x0020
        // OS type:         0x0001
    }
}
