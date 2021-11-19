// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static MacIO.Conversion;

namespace MacIO;

public class BootBlockHeader : Record
{
    // Valid, Allocated, InUse, AllowsReading, AllowsWriting, Unused
    public const ushort ValidSignature = 0x4C4B; // "LK"

    // Logical blocks are always 512bytes inside an HFS partition.
    private readonly byte[] _data = new byte[1024];
    private ref BootBlkHdr HeaderData => ref Unsafe.As<byte, BootBlkHdr>(ref Unsafe.AsRef(_data[0]));

    private readonly long _startPosition;

    public BootBlockHeader(Stream stream, long position, bool readOnly = true)
        : base(stream, readOnly)
    {
        Stream.Seek(position, SeekOrigin.Begin);
        Stream.Read(_data);
        _startPosition = position;
    }

    /// <summary>
    ///  The book block signature ("LK").
    /// </summary>
    public ushort Signature
    {
        get => BigEndian(HeaderData.bbID);
        set => HeaderData.bbID = BigEndian(value);
    }

    /// <summary>
    ///  Name of the System file.
    /// </summary>
    public unsafe string SystemFileName
    {
        get
        {
            fixed (byte* b = HeaderData.bbSysName)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  Name of the shell file. Usually, the system shell is Finder.
    /// </summary>
    public unsafe string ShellFileName
    {
        get
        {
            fixed (byte* b = HeaderData.bbShellName)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  First debugger installed during the boot process. Typically this is Macsbug.
    /// </summary>
    public unsafe string FirstDebugger
    {
        get
        {
            fixed (byte* b = HeaderData.bbDbg1Name)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  Second debugger installed during the boot process. Typically this is Disassembler.
    /// </summary>
    public unsafe string SecondDebugger
    {
        get
        {
            fixed (byte* b = HeaderData.bbDbg2Name)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  File containing the startup screen. Usually this is StartUpScreen.
    /// </summary>
    public unsafe string StartupScreen
    {
        get
        {
            fixed (byte* b = HeaderData.bbScreenName)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  The name of the startup program. Usually this is Finder.
    /// </summary>
    public unsafe string StartupProgram
    {
        get
        {
            fixed (byte* b = HeaderData.bbHelloName)
                return GetPascalString(new(b, 16));
        }
    }

    /// <summary>
    ///  Name of the system scrap file. Usually this is Clipboard.
    /// </summary>
    public unsafe string ScrapName
    {
        get
        {
            fixed (byte* b = HeaderData.bbScrapName)
            {
                return GetPascalString(new(b, 16));
            }
        }
    }

    public override void Save()
    {
        Stream.Seek(_startPosition, SeekOrigin.Begin);
        Stream.Write(_data);
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    private unsafe struct BootBlkHdr
    {
        public ushort bbID;                     // boot blocks signature
        public uint bbEntry;                    // entry point to boot code
        public ushort bbVersion;                // boot blocks version number
        public ushort bbPageFlags;              // used internally
        public fixed byte bbSysName[16];        // System filename [Str15]
        public fixed byte bbShellName[16];      // Finder filename [Str15]
        public fixed byte bbDbg1Name[16];       // debugger filename [Str15]
        public fixed byte bbDbg2Name[16];       // debugger filename [Str15]
        public fixed byte bbScreenName[16];     // name of startup screen [Str15]
        public fixed byte bbHelloName[16];      // name of startup program [Str15]
        public fixed byte bbScrapName[16];      // name of system scrap file [Str15]
        public ushort bbCntFCBs;                // number of FCBs to allocate
        public ushort bbCntEvts;                // number of event queue elements
        public uint bb128KSHeap;                // system heap size on 128K Mac
        public uint bb256KSHeap;                // used internally
        public uint bbSysHeapSize;              // system heap size on all machines
        public ushort filler;                   // reserved
        public uint bbSysHeapExtra;             // additional system heap space
        public uint bbSysHeapFract;             // fraction of RAM for system heap
    }
}
