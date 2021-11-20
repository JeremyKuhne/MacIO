// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public partial class ProDOSVolume
{
    public enum StorageType : byte
    {
        SeedingFile                 = 0x01,
        SaplingFile                 = 0x02,
        TreeFile                    = 0x03,
        PascalArea                  = 0x04,
        Subdirectory                = 0xD0,
        VolumeDirectoryFileKeyBlock = 0xF0,
        SubDirectoryFileKeyBlock    = 0xE0
    }
}

