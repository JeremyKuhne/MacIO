// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using static MacIO.Conversion;

namespace MacIO;

public class MasterDirectoryBlock : Record
{
    public const ushort ValidSignature  = 0x4244; // "BD"
    public const ushort OldSignature    = 0xD2D7;

    private MDB _data;

    private readonly long _location;

    public MasterDirectoryBlock(Stream stream, long location, bool readOnly = true)
        : base(stream, readOnly)
    {
        _location = location;
        Stream.Seek(location, SeekOrigin.Begin);
        Stream.ReadStruct(ref _data);
    }

    /// <summary>
    ///  The master directory signature ("BD").
    /// </summary>
    public ushort Signature
    {
        get => BigEndian(_data.drSigWord);
        set => _data.drSigWord = BigEndian(value);
    }

    public DateTime VolumeCreationTime
    {
        get => GetHFSDate(BigEndian(_data.drCrDate));
    }

    public DateTime LastModificationTime
    {
        get => GetHFSDate(BigEndian(_data.drLsMod));
    }

    public VolumeAttributes VolumeAttributes
    {
        get => (VolumeAttributes)BigEndian(_data.drAtrb);
    }

    public ushort NumberOfRootFiles
    {
        get => BigEndian(_data.drNmFls);
    }

    // Should always be 3
    public ushort VolumeBitmapFirstBlock
    {
        get => BigEndian(_data.drVBMSt);
    }

    public ushort AllocationSearchBlock
    {
        get => BigEndian(_data.drAllocPtr);
    }

    public uint NumberOfAllocationBlocks
    {
        get => BigEndian(_data.drNmAlBlks);
    }

    /// <summary>
    ///  In bytes. Must be a multiple of 512.
    /// </summary>
    public uint AllocationBlockSize
    {
        get => BigEndian(_data.drAlBlkSiz);
    }

    public ushort FirstAllocationBlock
    {
        get => BigEndian(_data.drAlBlSt);
    }

    public uint UnusedAllocationBlocks
    {
        get => BigEndian(_data.drFreeBks);
    }

    public uint DefaultClumpSize
    {
        get => BigEndian(_data.drClpSiz);
    }

    public uint NextUnusedCatalogNodeId
    {
        get => BigEndian(_data.drNxtCNID);
    }

    public unsafe string VolumeName
    {
        get
        {
            fixed (byte* b = _data.drVN)
                return GetPascalString(new(b, 28));
        }
    }

    public DateTime LastBackup
    {
        get => GetHFSDate(BigEndian(_data.drVolBkUp));
    }

    public uint BackupSequenceNumber
    {
        get => BigEndian(_data.drVSeqNum);
    }

    public uint VolumeWriteCount
    {
        get => BigEndian(_data.drWrCnt);
    }

    public uint ExtentsClumpSize
    {
        get => BigEndian(_data.drXTClpSiz);
    }

    public uint CatalogClumpSize
    {
        get => BigEndian(_data.drCTClpSiz);
    }

    public ushort NumberOfRootDirectories
    {
        get => BigEndian(_data.drNmRtDirs);
    }

    public uint NumberOfFiles
    {
        get => BigEndian(_data.drFilCnt);
    }

    public uint NumberOfDirectories
    {
        get => BigEndian(_data.drDirCnt);
    }

    public ushort VolumeCacheSize
    {
        get => BigEndian(_data.drVCSize);
    }

    public ushort VolumeBitmapCacheSize
    {
        get => BigEndian(_data.drVBMCSize);
    }

    public ushort CommonVolumeCacheSize
    {
        get => BigEndian(_data.drCtlCSize);
    }

    public uint ExtentsOverflowFileSize
    {
        get => BigEndian(_data.drXTFlSize);
    }

    public uint CatalogFileSize
    {
        get => BigEndian(_data.drCTFlSize);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private unsafe struct MDB
    {
        public ushort drSigWord;        // volume signature
        public uint drCrDate;           // date and time of volume creation
        public uint drLsMod;            // date and time of last modification
        public ushort drAtrb;           // volume attributes
        public ushort drNmFls;          // number of files in root directory
        public ushort drVBMSt;          // first block of volume bitmap
        public ushort drAllocPtr;       // start of next allocation search
        public ushort drNmAlBlks;       // number of allocation blocks in volume
        public uint drAlBlkSiz;         // size(in bytes) of allocation blocks
        public uint drClpSiz;           // default clump size
        public ushort drAlBlSt;         // first allocation block in volume
        public uint drNxtCNID;          // next unused catalog node ID
        public ushort drFreeBks;        // number of unused allocation blocks
        public fixed byte drVN[28];     // volume name String[27]
        public uint drVolBkUp;          // date and time of last backup
        public ushort drVSeqNum;        // volume backup sequence number
        public uint drWrCnt;            // volume write count
        public uint drXTClpSiz;         // clump size for extents overflow file
        public uint drCTClpSiz;         // clump size for catalog file
        public ushort drNmRtDirs;       // number of directories in root directory
        public uint drFilCnt;           // number of files in volume
        public uint drDirCnt;           // number of directories in volume
        public FndrInfo drFndrInfo;     // information used by the Finder
        public ushort drVCSize;         // size(in blocks) of volume cache
        public ushort drVBMCSize;       // size(in blocks) of volume bitmap cache
        public ushort drCtlCSize;       // size(in blocks) of common volume cache
        public uint drXTFlSize;         // size of extents overflow file
        public ExtDataRec drXTExtRec;   // extent record for extents overflow file
        public uint drCTFlSize;         // size of catalog file
        public ExtDataRec drCTExtRec;   // extent record for catalog file
    }

    public unsafe override void Save()
    {
        Stream.Seek(_location, SeekOrigin.Begin);
        fixed (void* d = &_data)
        {
            Stream.Write(new ReadOnlySpan<byte>(d, sizeof(FndrInfo)));
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct FndrInfo
    {
        public uint SystemFolderDirectoryId;
        public uint StartupApplicationParentId;
        public uint OpenDirectoryId;
        public uint MacOSSystemFolderDirectoryId;
        public uint Reserved;
        public uint OSXSystemFolderDirectoryId;
        public uint OSXVolumeID1;
        public uint OSXVolumeID2;
    }
}
