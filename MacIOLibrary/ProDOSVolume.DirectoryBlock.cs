// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MacIO;

public partial class ProDOSVolume
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private unsafe struct DirectoryBlock
    {
        public ushort priorBlock;
        public ushort nextBlock;
    }
}

