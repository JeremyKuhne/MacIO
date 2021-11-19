// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TestProject;

public class EmptyMac100MBHD : ImageFixture
{
    public EmptyMac100MBHD() : base(@"TestAssets\MacHD 100MB HFS.hda.zip")
    {
    }
}

public class EmptyHFS5MBHD : ImageFixture
{
    public EmptyHFS5MBHD() : base(@"TestAssets\FormattedEmptyHFS5MB.zip")
    {
    }
}
