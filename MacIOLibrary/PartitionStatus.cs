// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

[Flags]
public enum PartitionStatus : uint
{
    Valid                           = 0b0000_0001,
    Allocated                       = 0b0000_0010,
    InUse                           = 0b0000_0100,
    ValidBootInfo                   = 0b0000_1000,
    AllowsReading                   = 0b0001_0000,
    AllowsWriting                   = 0b0010_0000,
    BootCodePositionIndependent     = 0b0100_0000,

    // Documentation claims it is unused, but actually seeing this.
    Unused                          = 0b1000_0000
}
