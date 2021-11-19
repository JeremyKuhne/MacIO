// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace MacIO;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ExtDataRec
{
    public ExtDescriptor extDescriptor1;
    public ExtDescriptor extDescriptor2;
    public ExtDescriptor extDescriptor3;
}
