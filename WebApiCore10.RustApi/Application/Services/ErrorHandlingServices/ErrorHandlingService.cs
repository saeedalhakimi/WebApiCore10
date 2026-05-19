using DoimanDlls.Exceptions;
using SharedDll.Errors;
using SharedDll.Results;

namespace WebApiCore10.RustApi.Application.Services.ErrorHandlingServices
{
    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly ILogger<ErrorHandlingService> _logger;
        public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
        {
            _logger = logger;
        }

        public OperationResult<T> HandleCancelationTokenException<T>(OperationCanceledException ex, string operation, string correlationId)
        {
            _logger.LogWarning(ex, "Operation {Operation} was canceled. CorrelationId: {CorrelationId}", operation, correlationId);
            return OperationResult<T>.Failure(Error.FromException(ErrorCode.OperationCanceled, ex, operation, correlationId));
        }

        public OperationResult<T> HandleDomainException<T>(DomainException ex, string operation, string correlationId)
        {
            _logger.LogError(ex, "A domain error occurred during {Operation}. CorrelationId: {CorrelationId}", operation, correlationId);
            return OperationResult<T>.Failure(Error.FromException(ErrorCode.DomainValidationError, ex, operation, correlationId));
        }

        public OperationResult<T> HandleResourceConflictError<T>(string resource, string correlationId)
        {
            var errorMessage = $"Resource with: {resource} already exists.";
            _logger.LogWarning(errorMessage);
            return OperationResult<T>
                .Failure(
                    new Error(
                        ErrorCode.Conflict,
                        details: errorMessage,
                        correlationId: correlationId));
        }

        public OperationResult<T> HandleResourceCreationFailed<T>(string resourceName, string operationName,
            string? failureDetails, string correlationId)
        {
            var logMessage =
                "Operation '{Operation}' failed while creating resource to '{Resource}'. " +
                "Details: {Details}. CorrelationId: {CorrelationId}";

            _logger.LogError(logMessage, operationName, resourceName,
                failureDetails ?? "No additional details provided", correlationId);

            return OperationResult<T>.Failure(
                new Error(
                    ErrorCode.ResourceCreationFailed,
                    details: $"Failed to create '{resourceName}'.",
                    correlationId: correlationId));
        }

        public OperationResult<T> HandleUnknownExceptions<T>(Exception ex, string operation, string correlationId)
        {
            _logger.LogError(ex, "An unexpected error occurred during {Operation}. CorrelationId: {CorrelationId}", operation, correlationId);
            return OperationResult<T>.Failure(Error.FromException(ErrorCode.UnknownError, ex, operation, correlationId));
        }
    }
}
