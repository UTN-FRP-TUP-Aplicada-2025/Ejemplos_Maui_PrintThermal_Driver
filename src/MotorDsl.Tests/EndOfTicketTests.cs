using MotorDsl.Core.Models;
using MotorDsl.Rendering;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre la cola del ticket (capability end_of_ticket) y el mapeo de PrintNotConfirmedException.
/// El default NO es corte total: GS V 0 hace avanzar 12-13 cm de papel en las termicas sin
/// cortadora (medido en MTP-II y 58HB6). Ver 13-Impresion-Logo.
/// </summary>
public class EndOfTicketTests
{
    [Fact]
    public void PorDefecto_EsCorteParcial_NoCorteTotal()
    {
        Assert.Equal(EscPosCommands.CutPartial, EscPosCommands.EndOfTicket(null));
        Assert.NotEqual(EscPosCommands.CutFull, EscPosCommands.EndOfTicket(null));
    }

    [Theory]
    [InlineData("cut_partial")]
    [InlineData("valor-desconocido")]
    [InlineData("")]
    public void ModoDesconocido_CaeEnElDefault(string modo)
        => Assert.Equal(EscPosCommands.CutPartial, EscPosCommands.EndOfTicket(modo));

    [Fact]
    public void CutFull_EmiteGsV0()
        => Assert.Equal(EscPosCommands.CutFull, EscPosCommands.EndOfTicket("cut_full"));

    [Fact]
    public void None_NoEmiteNada()
        => Assert.Empty(EscPosCommands.EndOfTicket("none"));

    [Fact]
    public void Feed_EmiteEscDn()
        => Assert.Equal(new byte[] { 0x1B, 0x64, 6 }, EscPosCommands.EndOfTicket("feed", 6));

    [Fact]
    public void Feed_ConCeroLineas_NoEmiteNada()
        => Assert.Empty(EscPosCommands.EndOfTicket("feed", 0));

    [Fact]
    public void Feed_SeAcotaA255()
        => Assert.Equal(new byte[] { 0x1B, 0x64, 255 }, EscPosCommands.EndOfTicket("feed", 9999));

    // ── Confirmacion de impresion ──

    [Fact]
    public void PrintNotConfirmed_SeClasificaComoProtocol_YNoSeReintenta()
    {
        var ex = new PrintNotConfirmedException("sin confirmacion");
        var error = PrintError.FromException(ex, attempt: 1, maxAttempts: 3);

        // Protocol: el documento pudo imprimirse parcialmente, no se reimprime solo.
        Assert.Equal(PrintErrorType.Protocol, error.Type);
    }
}
