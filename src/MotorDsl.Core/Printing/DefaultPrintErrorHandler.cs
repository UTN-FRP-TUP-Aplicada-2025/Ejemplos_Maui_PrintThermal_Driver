using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Printing;

/// <summary>
/// Default implementation of IPrintErrorHandler.
/// Retries on Connection and Timeout errors, aborts on Hardware and Protocol.
/// Methods are virtual so consumers can override specific behaviors.
/// 
/// Sprint 06 | TK-40
/// Supports: CU-32
/// </summary>
public class DefaultPrintErrorHandler : IPrintErrorHandler
{
    public virtual Task<bool> HandleErrorAsync(PrintError error)
    {
        var shouldRetry = error.Type switch
        {
            PrintErrorType.Connection => true,
            PrintErrorType.Timeout => true,
            PrintErrorType.Hardware => false,
            PrintErrorType.Protocol => false,
            _ => true
        };

        return Task.FromResult(shouldRetry);
    }

    public virtual void OnRetryAttempt(PrintError error)
    {
        // No-op by default. Override to show UI or log.
    }

    public virtual void OnPrintSuccess(int totalAttempts)
    {
        // No-op by default. Override to show UI or log.
    }
}
