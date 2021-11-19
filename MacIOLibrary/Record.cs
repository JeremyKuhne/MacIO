// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MacIO;

public abstract class Record
{
    /// <summary>
    ///  The stream backing the record.
    /// </summary>
    protected Stream Stream { get; }

    /// <summary>
    ///  True if the stream is read only.
    /// </summary>
    protected bool ReadOnly { get; }

    protected Record(Stream stream, bool readOnly = true)
    {
        Stream = readOnly && stream.CanWrite ? new ReadOnlyStream(stream) : stream;
        ReadOnly = readOnly;
    }

    /// <summary>
    ///  Persist back to the stream backing the record.
    /// </summary>
    public abstract void Save();
}
