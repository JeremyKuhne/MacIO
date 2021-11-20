// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static MacIO.Conversion;

namespace MacIO;

/// <summary>
///  The partition map is a data structure that describes the partitions present on a block
///  device. The Macintosh Operating System and all other operating systems from Apple
///  use the same partitioning method. This allows a single device to support multiple
///  operating systems.
/// </summary>
/// <remarks>
///  <para>
///   The partition map always begins at physical block 1, the second block on the disk. With
///   the exception of the driver descriptor record in block 0, every block on a disk must
///   belong to a partition.
///  </para>
///  <para>
///   Each partition on a disk is described by an entry in the partition map. The partition map
///   is itself a partition, and contains an entry describing itself. The partition map entry for
///   the partition map is not necessarily the first entry in the map. Partition map entries can
///   be in any order, and need not correspond to the physical organization of partitions on
///   the disk.
///  </para>
///  <para>
///   The number of entries in the partition map is not restricted. However, because the
///   partition map must begin at block 1 and must be contiguous, it cannot easily be
///   expanded once other partitions are created. One way around this limitation is to
///   create a large number of empty partition map entries when the disk is initialized.
///  </para>
///  <para>
///   To locate a partition, the Start Manager examines the pmMapBlkCnt field of the first
///   partition map entry.This field contains the size of the partition map, in blocks. Then,
///   using the block size value from the sbBlkSize field of the driver descriptor record, the
///   Start Manager reads each block in the partition map, looking for a valid signature in the
///   pmSIG field of each partition map entry record.
///  </para>
/// </remarks>
public class PartitionMapEntry : Record
{
    public const ushort ValidSignature = 0x504D; // "PM"
    public const ushort OldSignature = 0x5453;

    private readonly byte[] _data;
    private ref Partition PartitionData => ref Unsafe.As<byte, Partition>(ref Unsafe.AsRef(_data[0]));

    private int PhysicalBlockSize { get; }

    private int _index;

    public PartitionMapEntry(Stream stream, int index, bool readOnly = true)
        : base(stream, readOnly)
    {
        _index = index;

        BlockZero blockZero = new(stream);
        PhysicalBlockSize = blockZero.BlockSize;

        if (PhysicalBlockSize == 0)
        {
            throw new InvalidOperationException("The image block size is zero.");
        }

        _data = new byte[PhysicalBlockSize];

        // First entry starts at physical block 1
        long position = PhysicalBlockSize + index * PhysicalBlockSize;
        if (Stream.CanWrite && Stream.Length < position + PhysicalBlockSize)
        {
            Stream.SetLength(position + PhysicalBlockSize);
        }

        Stream.Seek(position, SeekOrigin.Begin);
        stream.Read(_data);
    }

    // "Apple_partition_map"
    private static ReadOnlySpan<byte> PartitionMap => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'p', (byte)'a', (byte)'r', (byte)'t', (byte)'i', (byte)'t', (byte)'i', (byte)'o', (byte)'n', (byte)'_',
            (byte)'m', (byte)'a', (byte)'p', (byte)'\0'
        };

    // "Apple_Driver"
    private static ReadOnlySpan<byte> DeviceDriver => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'D', (byte)'r', (byte)'i', (byte)'v', (byte)'e', (byte)'r', (byte)'\0'
        };

    // "Apple_Driver43"
    private static ReadOnlySpan<byte> DeviceDriver43 => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'D', (byte)'r', (byte)'i', (byte)'v', (byte)'e', (byte)'r',  (byte)'4', (byte)'3', (byte)'\0'
        };

    // "Apple_MFS"
    private static ReadOnlySpan<byte> MacintoshFileSystem => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'M', (byte)'F', (byte)'S', (byte)'\0'
        };

    // "Apple_HFS"
    private static ReadOnlySpan<byte> HierarchicalFileSystem => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'H', (byte)'F', (byte)'S', (byte)'\0'
        };

    // "Apple_Unix_SVR2"
    private static ReadOnlySpan<byte> UnixFileSystem => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'U', (byte)'n', (byte)'i', (byte)'x', (byte)'_',
            (byte)'S', (byte)'V', (byte)'R', (byte)'2', (byte)'\0'
        };

    // "Apple_PRODOS"
    private static ReadOnlySpan<byte> ProDOSFileSystem => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'P', (byte)'R', (byte)'O', (byte)'D', (byte)'O', (byte)'S', (byte)'\0'
        };

    // "Apple_Free"
    private static ReadOnlySpan<byte> Unused => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'F', (byte)'r', (byte)'e', (byte)'e', (byte)'\0'
        };

    // "Apple_Scratch"
    private static ReadOnlySpan<byte> Empty => new byte[]
        {
            (byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e', (byte)'_',
            (byte)'S', (byte)'c', (byte)'r', (byte)'a', (byte)'t', (byte)'c',  (byte)'h', (byte)'\0'
        };

    private static ReadOnlySpan<byte> Motorola68000 => new byte[]
        {
            (byte)'6', (byte)'8', (byte)'0', (byte)'0', (byte)'0', (byte)'\0'
        };

    private static ReadOnlySpan<byte> Motorola68020 => new byte[]
        {
            (byte)'6', (byte)'8', (byte)'0', (byte)'2', (byte)'0', (byte)'\0'
        };

    private static ReadOnlySpan<byte> Motorola68030 => new byte[]
        {
            (byte)'6', (byte)'8', (byte)'0', (byte)'3', (byte)'0', (byte)'\0'
        };

    private static ReadOnlySpan<byte> Motorola68040 => new byte[]
        {
            (byte)'6', (byte)'8', (byte)'0', (byte)'4', (byte)'0', (byte)'\0'
        };


    /// <summary>
    ///  The partition signature ("PM").
    /// </summary>
    public ushort Signature
    {
        get => BigEndian(PartitionData.pmSig);
        set => PartitionData.pmSig = BigEndian(value);
    }

    /// <summary>
    ///  Reserved, must be 0.
    /// </summary>
    public ushort SignaturePadding
    {
        get => BigEndian(PartitionData.pmSigPad);
        set => PartitionData.pmSigPad = BigEndian(value);
    }

    /// <summary>
    ///  The number of entries in the partition map.
    /// </summary>
    /// <remarks>
    ///  This is the total number of partition entries, and must be the same in every entry.
    /// </remarks>
    public uint MapEntries
    {
        get => BigEndian(PartitionData.pmMapBlkCnt);
        set => PartitionData.pmMapBlkCnt = BigEndian(value);
    }

    /// <summary>
    ///  The physical block number of the first block of the partition.
    /// </summary>
    /// <example>
    ///  For the first partition map entry ("Apple_partition_map") this would always be '1' as
    ///  it must occupy the 1st block (0 based, follows the driver descriptor record in block 0).
    /// </example>
    public uint PhysicalStart
    {
        get => BigEndian(PartitionData.pmPyPartStart);
        set => PartitionData.pmPyPartStart = BigEndian(value);
    }

    /// <summary>
    ///  The size of the partition, in blocks.
    /// </summary>
    /// <remarks>
    ///  The first partition map entry describes the partition map itself, in that case it would
    ///  be the total amount of reserved space for partition entries.
    /// </remarks>
    public uint BlockCount
    {
        get => BigEndian(PartitionData.pmPartBlkCnt);
        set => PartitionData.pmPartBlkCnt = BigEndian(value);
    }

    /// <summary>
    ///  Optional name, up to 32 ASCII characters.
    /// </summary>
    /// <remarks>
    ///  If the name starts with "Maci" the  Start Manager will perform checksum validation of the
    ///  device driver's boot code.
    ///
    ///  Some observed names:
    ///
    ///   - "Apple" for the partition map entry.
    ///   - "Driver_Partition" for the driver partition.
    /// </remarks>
    public unsafe string Name
    {
        get
        {
            fixed (byte* b = PartitionData.pmPartName)
            {
                return FromNullTerminatedAsciiString(new(b, 32));
            }
        }
        set
        {
            fixed (byte* b = PartitionData.pmPartName)
            {
                Span<byte> span = new(b, 32);
                span.Clear();
                Encoding.ASCII.GetBytes(value, span);
            }
        }
    }

    /// <summary>
    ///  Partition type.
    /// </summary>
    public unsafe PartitionType Type
    {
        get
        {
            fixed (byte* b = PartitionData.pmParType)
            {
                ReadOnlySpan<byte> span = new(b, 32);
                if (span.StartsWith(HierarchicalFileSystem))
                    return PartitionType.HierarchicalFileSystem;
                else if (span.StartsWith(ProDOSFileSystem))
                    return PartitionType.ProDOSFileSystem;
                else if (span.StartsWith(PartitionMap))
                    return PartitionType.PartitionMap;
                else if (span.StartsWith(DeviceDriver))
                    return PartitionType.DeviceDriver;
                else if (span.StartsWith(DeviceDriver43))
                    return PartitionType.DeviceDriver43;
                else if (span.StartsWith(Empty))
                    return PartitionType.Empty;
                else if (span.StartsWith(Unused))
                    return PartitionType.Unused;
                else if (span.StartsWith(MacintoshFileSystem))
                    return PartitionType.MacintoshFileSystem;
                else if (span.StartsWith(UnixFileSystem))
                    return PartitionType.UnixFileSystem;
                else
                    return PartitionType.Unknown;
            }
        }
        set
        {
            fixed (byte* b = PartitionData.pmParType)
            {
                Span<byte> span = new(b, 32);
                span.Clear();
                switch (value)
                {
                    case PartitionType.PartitionMap:
                        PartitionMap.CopyTo(span);
                        break;
                    case PartitionType.DeviceDriver:
                        DeviceDriver.CopyTo(span);
                        break;
                    case PartitionType.MacintoshFileSystem:
                        MacintoshFileSystem.CopyTo(span);
                        break;
                    case PartitionType.HierarchicalFileSystem:
                        HierarchicalFileSystem.CopyTo(span);
                        break;
                    case PartitionType.ProDOSFileSystem:
                        ProDOSFileSystem.CopyTo(span);
                        break;
                    case PartitionType.DeviceDriver43:
                        DeviceDriver43.CopyTo(span);
                        break;
                    case PartitionType.UnixFileSystem:
                        UnixFileSystem.CopyTo(span);
                        break;
                    case PartitionType.Unused:
                        Unused.CopyTo(span);
                        break;
                    case PartitionType.Empty:
                        Empty.CopyTo(span);
                        break;
                    case PartitionType.Unknown:
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    ///  Partition type, up to 32 ASCII characters.
    /// </summary>
    public unsafe string TypeString
    {
        get
        {
            fixed (byte* b = PartitionData.pmParType)
                return FromNullTerminatedAsciiString(new(b, 32));
        }
        set
        {
            fixed (byte* b = PartitionData.pmParType)
            {
                Span<byte> span = new(b, 32);
                span.Clear();
                Encoding.ASCII.GetBytes(value, span);
            }
        }
    }

    /// <summary>
    ///  First block containing file system data. Normally 0 outside of A/UX.
    /// </summary>
    public uint LogicalDataStart
    {
        get => BigEndian(PartitionData.pmLgDataStart);
        set => PartitionData.pmLgDataStart = BigEndian(value);
    }

    /// <summary>
    ///  File system data area size in blocks.
    /// </summary>
    public uint DataCount
    {
        get => BigEndian(PartitionData.pmDataCnt);
        set => PartitionData.pmDataCnt = BigEndian(value);
    }

    /// <summary>
    ///  Status, only used by A/UX.
    /// </summary>
    /// <remarks>
    ///  Although this is only used by A/UX, in practice it appears to always be filled out with 0x33
    ///  (valid, allocated, readable and writable).
    /// </remarks>
    public PartitionStatus Status
    {
        get => (PartitionStatus)BigEndian(PartitionData.pmPartStatus);
        set => PartitionData.pmPartStatus = BigEndian((uint)value);
    }

    /// <summary>
    ///  Logical block number of the first block containing boot code.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public uint LogicalBootStart
    {
        get => BigEndian(PartitionData.pmLgBootStart);
        set => PartitionData.pmLgBootStart = BigEndian(value);
    }

    /// <summary>
    ///  Size of the boot code in bytes.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public uint BootSize
    {
        get => BigEndian(PartitionData.pmBootSize);
        set => PartitionData.pmBootSize = BigEndian(value);
    }

    /// <summary>
    ///  Address where boot code is to be loaded.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public uint BootAddress
    {
        get => BigEndian(PartitionData.pmBootAddr);
        set => PartitionData.pmBootAddr = BigEndian(value);
    }

    /// <summary>
    ///  Reserved.
    /// </summary>
    public uint BootAddress2
    {
        get => BigEndian(PartitionData.pmBootAddr2);
        set => PartitionData.pmBootAddr2 = BigEndian(value);
    }

    /// <summary>
    ///  The memory address to which the Start Manager will transfer control after
    ///  loading the boot code into memory.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public uint BootEntry
    {
        get => BigEndian(PartitionData.pmBootEntry);
        set => PartitionData.pmBootEntry = BigEndian(value);
    }

    /// <summary>
    ///  Reserved.
    /// </summary>
    public uint BootEntry2
    {
        get => BigEndian(PartitionData.pmBootEntry2);
        set => PartitionData.pmBootEntry2 = BigEndian(value);
    }

    /// <summary>
    ///  Checksum of the boot code.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public uint BootChecksum
    {
        get => BigEndian(PartitionData.pmBootCksum);
        set => PartitionData.pmBootCksum = BigEndian(value);
    }

    /// <summary>
    ///  Processor type string for the boot code.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public unsafe string ProcessorString
    {
        get
        {
            fixed (byte* b = PartitionData.pmProcessor)
            {
                return FromNullTerminatedAsciiString(new(b, 16));
            }
        }
        set
        {
            fixed (byte* b = PartitionData.pmProcessor)
            {
                Span<byte> span = new(b, 16);
                span.Clear();
                Encoding.ASCII.GetBytes(value, span);
            }
        }
    }

    /// <summary>
    ///  Processor type for the boot code.
    /// </summary>
    /// <remarks>
    ///  Only relevant for device driver partitions.
    /// </remarks>
    public unsafe ProcessorType Processor
    {
        get
        {
            fixed (byte* b = PartitionData.pmProcessor)
            {
                ReadOnlySpan<byte> span = new(b, 16);
                if (span.StartsWith(Motorola68000))
                {
                    return ProcessorType.Motorola68000;
                }
                else if (span.StartsWith(Motorola68020))
                {
                    return ProcessorType.Motorola68020;
                }
                else if (span.StartsWith(Motorola68030))
                {
                    return ProcessorType.Motorola68030;
                }
                else if (span.StartsWith(Motorola68040))
                {
                    return ProcessorType.Motorola68040;
                }
                else
                {
                    return ProcessorType.Unknown;
                }
            }
        }
        set
        {
            fixed (byte* b = PartitionData.pmProcessor)
            {
                Span<byte> span = new(b, 16);
                span.Clear();
                switch (value)
                {
                    case ProcessorType.Motorola68000:
                        Motorola68000.CopyTo(span);
                        break;
                    case ProcessorType.Motorola68020:
                        Motorola68020.CopyTo(span);
                        break;
                    case ProcessorType.Motorola68030:
                        Motorola68030.CopyTo(span);
                        break;
                    case ProcessorType.Motorola68040:
                        Motorola68040.CopyTo(span);
                        break;
                    case ProcessorType.Unknown:
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    ///  "Padding" bytes at the end of the partition entry.
    /// </summary>
    /// <remarks>
    ///  This is documented as reserved. It is known to be used for driver partition entries.
    /// </remarks>
    public Span<byte> PaddingBytes => new(_data, _data.Length - sizeof(ushort) * 188, sizeof(ushort) * 188);

    public override void Save()
    {
        long position = _index * PhysicalBlockSize + PhysicalBlockSize;
        Stream.Seek(position, SeekOrigin.Begin);
        Stream.Write(_data);
    }

    /// <summary>
    ///  Set a new index for this map entry.
    /// </summary>
    public void SetIndex(int index) => _index = index;

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct Partition
    {
        public ushort pmSig;
        public ushort pmSigPad;
        public uint pmMapBlkCnt;
        public uint pmPyPartStart;
        public uint pmPartBlkCnt;
        public fixed byte pmPartName[32];
        public fixed byte pmParType[32];
        public uint pmLgDataStart;
        public uint pmDataCnt;
        public uint pmPartStatus;
        public uint pmLgBootStart;
        public uint pmBootSize;
        public uint pmBootAddr;
        public uint pmBootAddr2;
        public uint pmBootEntry;
        public uint pmBootEntry2;
        public uint pmBootCksum;
        public fixed byte pmProcessor[16];
        // 188 shorts of padding follow
    }
}
