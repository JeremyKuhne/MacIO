// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace xTask.Logging;

using xTask.Utility;
using System.Text;

public class TextLogger : TextTableLogger
{
    private readonly StringBuilder _text = new(4096);

    protected override int TableWidth => 120;

    protected override void WriteInternal(WriteStyle style, string value)
    {
        if (style.HasFlag(WriteStyle.Underline))
        {
            _text.Append(Strings.Underline(value));
        }
        else
        {
            _text.Append(value);
        }
    }

    public override string ToString() => _text.ToString();
}
