using System;

namespace ArchestrA.Apps.MPNZ.AttributeBrowserApp
{
    [Flags]
    public enum Filter
    {
        IsStatus = 0x01,
        IsProcessValue = 0x02,
        IsCommand = 0x04,
        IsConfiguration = 0x08,
        IsCalculation = 0x10,
        HasInput = 0x20,
        HasOutput = 0x40,
        IsHidden = 0x80,
    }
}
