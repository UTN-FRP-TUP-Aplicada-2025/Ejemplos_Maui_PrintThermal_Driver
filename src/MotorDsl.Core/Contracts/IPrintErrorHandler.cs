using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for handling print errors with retry/abort decisions.
/// Consumers can implement this to customize error handling behavior,
/// show UI alerts, or log errors.
/// 
/// Sprint 06 | TK-40
/// Supports: CU-32
/// </summary>
public interface IPrintErrorHandler
{
    /// <summary>
    /// Decides whether to retry after a print error.
    /// Returns true to retry, false to abort.
    /// </summary>
    Task<bool> HandleErrorAsync(PrintError error);

    /// <summary>
    /// Called before each retry attempt, allowing UI updates or logging.
    /// </summary>
    void OnRetryAttempt(PrintError error);

    /// <summary>
    /// Called when printing succeeds (possibly after retries).
    /// </summary>
    void OnPrintSuccess(int totalAttempts);
}
