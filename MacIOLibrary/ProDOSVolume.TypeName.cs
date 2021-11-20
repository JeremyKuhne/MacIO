// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;

namespace MacIO;

public partial class ProDOSVolume
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private unsafe struct TypeName
    {
                                                    // Sample data from VolumeHeader
        public byte storage_type_and_namelength;    // F8 -> VolumeDirectoryFileKeyBlock, 8 characters
        public fixed byte file_name[15];            // 4D5950524F444F5300000000000000 // MYPRODOS

        public byte NameLength
        {
            get => (byte)(storage_type_and_namelength & 0b1111);
            set
            {
                if (value > 0b1111) // 15
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                storage_type_and_namelength = (byte)((storage_type_and_namelength & 0b1111_0000) | value);
            }
        }

        public StorageType StorageType
        {
            get => (StorageType)(storage_type_and_namelength & 0xF0);
            set => storage_type_and_namelength = (byte)((storage_type_and_namelength & 0x0F) | (byte)value);
        }

        public string Name
        {
            get
            {
                fixed(byte* b = file_name)
                    return Encoding.ASCII.GetString(new ReadOnlySpan<byte>(b, NameLength));
            }
            set
            {
                fixed (byte* b = file_name)
                {
                    Span<byte> span = new(b, 15);
                    span.Clear();
                    NameLength = (byte)Encoding.ASCII.GetBytes(value, span);
                }
            }
        }
    }
}

