// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public enum PartitionType
{
    Unknown,

    /// <summary>
    ///  Partition contains a partition map.
    /// </summary>
    PartitionMap,

    /// <summary>
    ///  Partition contains a device driver.
    /// </summary>
    DeviceDriver,

    /// <summary>
    ///  Partition contains a SCSI Manager 4.3 device driver.
    /// </summary>
    DeviceDriver43,

    /// <summary>
    ///  Partition uses the original Macintosh File System (64K ROM version).
    /// </summary>
    MacintoshFileSystem,

    /// <summary>
    ///  Partition uses the Hierarchical File System implemented in 128K and
    //   later ROM versions.
    /// </summary>
    HierarchicalFileSystem,

    /// <summary>
    ///  Partition uses the Unix file system.
    /// </summary>
    UnixFileSystem,

    /// <summary>
    ///  Partition uses the ProDOS file system.
    /// </summary>
    ProDOSFileSystem,

    /// <summary>
    ///  Partition is unused.
    /// </summary>
    Unused,

    /// <summary>
    ///  Partition is empty.
    /// </summary>
    Empty,
}
