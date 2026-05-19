using DoimanDlls.Exceptions;
using SharedDll.Results;

namespace WebApiCore10.RustApi.Application.Services.ErrorHandlingServices
{
    public interface IErrorHandlingService
    {
        OperationResult<T> HandleUnknownExceptions<T>(Exception ex, string operation, string correlationId);
        OperationResult<T> HandleCancelationTokenException<T>(OperationCanceledException ex, string operation, string correlationId);
        OperationResult<T> HandleDomainException<T>(DomainException ex, string operation, string correlationId);
        OperationResult<T> HandleResourceConflictError<T>(string resource, string correlationId);
        OperationResult<T> HandleResourceCreationFailed<T>(string resourceName, string operationName, string? failureDetails, string correlationId);
    }
}
