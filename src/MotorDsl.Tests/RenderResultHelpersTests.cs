using MotorDsl.Core.Models;

namespace MotorDsl.Tests;

/// <summary>
/// Tests for RenderResult helper methods: ToHexString() and ToBase64().
/// Sprint 05 — TK-31
/// 
/// ToHexString(): converts byte[] Output to "1B 40 48 6F 6C 61" format
/// ToBase64():    converts byte[] Output to Base64 string
/// Both return null if Output is not byte[].
/// </summary>
public class RenderResultHelpersTests
{
    // ── ToHexString ──────────────────────────────────────────────

    [Fact]
    public void ToHexString_ByteArrayOutput_ReturnsHexWithSpaces()
    {
        // ESC @ H o l a LF GS V 0x00
        var bytes = new byte[] { 0x1B, 0x40, 0x48, 0x6F, 0x6C, 0x61, 0x0A, 0x1D, 0x56, 0x00 };
        var result = new RenderResult("escpos", bytes);

        var hex = result.ToHexString();

        Assert.Equal("1B 40 48 6F 6C 61 0A 1D 56 00", hex);
    }

    [Fact]
    public void ToHexString_EmptyByteArray_ReturnsEmptyString()
    {
        var result = new RenderResult("escpos", Array.Empty<byte>());

        var hex = result.ToHexString();

        Assert.Equal("", hex);
    }

    [Fact]
    public void ToHexString_StringOutput_ReturnsNull()
    {
        var result = new RenderResult("text", "Hola mundo");

        var hex = result.ToHexString();

        Assert.Null(hex);
    }

    [Fact]
    public void ToHexString_NullOutput_ReturnsNull()
    {
        var result = new RenderResult("escpos", null);

        var hex = result.ToHexString();

        Assert.Null(hex);
    }

    [Fact]
    public void ToHexString_SingleByte_ReturnsWithoutSpaces()
    {
        var result = new RenderResult("escpos", new byte[] { 0xFF });

        var hex = result.ToHexString();

        Assert.Equal("FF", hex);
    }

    // ── ToBase64 ─────────────────────────────────────────────────

    [Fact]
    public void ToBase64_ByteArrayOutput_ReturnsBase64String()
    {
        var bytes = new byte[] { 0x1B, 0x40, 0x48, 0x6F, 0x6C, 0x61, 0x0A, 0x1D, 0x56, 0x00 };
        var result = new RenderResult("escpos", bytes);

        var base64 = result.ToBase64();

        Assert.NotNull(base64);
        Assert.Equal(Convert.ToBase64String(bytes), base64);
    }

    [Fact]
    public void ToBase64_EmptyByteArray_ReturnsEmptyBase64()
    {
        var result = new RenderResult("escpos", Array.Empty<byte>());

        var base64 = result.ToBase64();

        Assert.Equal("", base64);
    }

    [Fact]
    public void ToBase64_StringOutput_ReturnsNull()
    {
        var result = new RenderResult("text", "Hola mundo");

        var base64 = result.ToBase64();

        Assert.Null(base64);
    }

    [Fact]
    public void ToBase64_NullOutput_ReturnsNull()
    {
        var result = new RenderResult("escpos", null);

        var base64 = result.ToBase64();

        Assert.Null(base64);
    }

    // ── Roundtrip ────────────────────────────────────────────────

    [Fact]
    public void ToBase64_Roundtrip_DecodesToOriginalBytes()
    {
        var originalBytes = new byte[] { 0x1B, 0x40, 0x48, 0x6F, 0x6C, 0x61, 0x0A, 0x1D, 0x56, 0x00 };
        var result = new RenderResult("escpos", originalBytes);

        var base64 = result.ToBase64();
        var decoded = Convert.FromBase64String(base64!);

        Assert.Equal(originalBytes, decoded);
    }
}
