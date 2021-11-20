// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MacIO;

public partial class ProDOSVolume
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private unsafe struct VolumeHeader
    {
        public TypeName type_and_name;
        public fixed byte reserved[8];              // 00000000000080AF // ??
        public ushort creation_date;                // 2771
        public ushort creation_time;                // 3B16
        public byte version;                        // 00 ProDOS 1.0
        public byte min_version;                    // 00 ProDOS 1.0
        public byte access;                         // E3 (11100011) All flags
        public byte entry_length;                   // 27 (39)
        public byte entries_per_block;              // 0D (13) 39 * 13 = 507, 4 bytes for prior/next
        public ushort file_count;                   // 0000
        public ushort bit_map_pointer;              // 0006 (starts at 0xC00 [0x6 * 0x200]
        public ushort total_blocks;                 // FFFE (* 0x200 gives 1FFFC00 [33,553,408] max bytes)
    }
}

