// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

[Flags]
public enum VolumeAttributes : ushort
{
    HardwareLocked          = 0b0000_0000_1000_0000,
    SuccessfullyUnmounted   = 0b0000_0001_0000_0000,
    BadBlocksSpared         = 0b0000_0010_0000_0000,
    SoftwareLocked          = 0b1000_0000_0000_0000
}
