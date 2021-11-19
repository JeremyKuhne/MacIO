// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MacIO;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ExtDescriptor
{
    public ushort xdrStABN;     // first allocation block
    public ushort xdrNumABlks;  // number of allocation blocks
}
