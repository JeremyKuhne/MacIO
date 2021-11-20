// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public partial class ProDOSVolume
{
    [Flags]
    public enum Access : byte
    {
        ReadEnable      = 0b0000_0001,
        WriteEnable     = 0b0000_0011,
        Backup          = 0b0010_0000,
        RenameEnable    = 0b0100_0000,
        DestroyEnable   = 0b1000_0000
    }
}

