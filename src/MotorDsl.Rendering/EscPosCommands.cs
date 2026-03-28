namespace MotorDsl.Rendering;

/// <summary>
/// ESC/POS command constants as byte arrays.
/// Sprint 04 | TK-21
/// Reference: ESC/POS Application Programming Guide
/// </summary>
public static class EscPosCommands
{
    /// <summary>ESC @ — Initialize printer</summary>
    public static readonly byte[] Init = { 0x1B, 0x40 };

    /// <summary>ESC a 0 — Align left</summary>
    public static readonly byte[] AlignLeft = { 0x1B, 0x61, 0x00 };

    /// <summary>ESC a 1 — Align center</summary>
    public static readonly byte[] AlignCenter = { 0x1B, 0x61, 0x01 };

    /// <summary>ESC a 2 — Align right</summary>
    public static readonly byte[] AlignRight = { 0x1B, 0x61, 0x02 };

    /// <summary>ESC ! 0 — Normal style (reset)</summary>
    public static readonly byte[] StyleNormal = { 0x1B, 0x21, 0x00 };

    /// <summary>ESC ! 8 — Bold on</summary>
    public static readonly byte[] StyleBold = { 0x1B, 0x21, 0x08 };

    /// <summary>GS V 0 — Full cut</summary>
    public static readonly byte[] CutFull = { 0x1D, 0x56, 0x00 };

    /// <summary>GS V 1 — Partial cut</summary>
    public static readonly byte[] CutPartial = { 0x1D, 0x56, 0x01 };

    /// <summary>LF — Line feed</summary>
    public static readonly byte[] LineFeed = { 0x0A };

    /// <summary>ESC d n — Feed n lines</summary>
    public static byte[] FeedLines(byte n) => new byte[] { 0x1B, 0x64, n };
}
