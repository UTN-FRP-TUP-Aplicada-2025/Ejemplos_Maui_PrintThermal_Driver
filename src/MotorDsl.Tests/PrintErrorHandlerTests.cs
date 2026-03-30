using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Core.Printing;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para PrintError, IPrintErrorHandler y PrintRetryOptions.
/// Sprint 06 | TK-40, TK-41, TK-42
/// 5 test cases.
/// </summary>
public class PrintErrorHandlerTests
{
    // ═══════════════════════════════════════════════════
    // Clasificación de errores
    // ═══════════════════════════════════════════════════

    [Fact]
    public void PrintError_IOException_ClassifiesAsConnection()
    {
        var ex = new System.IO.IOException("Connection lost");
        var error = PrintError.FromException(ex, attempt: 1, maxAttempts: 3);

        Assert.Equal(PrintErrorType.Connection, error.Type);
        Assert.Equal(1, error.Attempt);
        Assert.Equal(3, error.MaxAttempts);
        Assert.Same(ex, error.InnerException);
    }

    [Fact]
    public void PrintError_TimeoutException_ClassifiesAsTimeout()
    {
        var ex = new TimeoutException("Operation timed out");
        var error = PrintError.FromException(ex, attempt: 2, maxAttempts: 3);

        Assert.Equal(PrintErrorType.Timeout, error.Type);
        Assert.Equal(2, error.Attempt);
        Assert.Contains("timed out", error.Message);
    }

    // ═══════════════════════════════════════════════════
    // DefaultPrintErrorHandler
    // ═══════════════════════════════════════════════════

    [Fact]
    public async Task DefaultPrintErrorHandler_ConnectionError_ReturnsTrue()
    {
        var handler = new DefaultPrintErrorHandler();
        var error = new PrintError(PrintErrorType.Connection, "BT lost", null, 1, 3);

        var shouldRetry = await handler.HandleErrorAsync(error);

        Assert.True(shouldRetry);
    }

    [Fact]
    public async Task DefaultPrintErrorHandler_HardwareError_ReturnsFalse()
    {
        var handler = new DefaultPrintErrorHandler();
        var error = new PrintError(PrintErrorType.Hardware, "No paper", null, 1, 3);

        var shouldRetry = await handler.HandleErrorAsync(error);

        Assert.False(shouldRetry);
    }

    // ═══════════════════════════════════════════════════
    // PrintRetryOptions defaults
    // ═══════════════════════════════════════════════════

    [Fact]
    public void PrintRetryOptions_DefaultValues_MaxRetries3_Delay500()
    {
        var options = new PrintRetryOptions();

        Assert.Equal(3, options.MaxRetries);
        Assert.Equal(500, options.InitialDelayMs);
    }
}
