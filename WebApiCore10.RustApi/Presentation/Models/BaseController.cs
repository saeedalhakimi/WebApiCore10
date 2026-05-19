using Microsoft.AspNetCore.Mvc;
using SharedDll.Errors;
using SharedDll.Results;

namespace WebApiCore10.RustApi.Presentation.Models
{
    [ApiController]
    public abstract class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;

        protected BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected string CorrelationId =>
            HttpContext.Items["CorrelationId"]?.ToString() ?? HttpContext.TraceIdentifier;

        protected IDisposable BeginRequestScope()
        {
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            _logger.LogInformation(
                "Request started: {Action} {Method} {Path} CorrelationId: {CorrelationId}",
                actionName,
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                CorrelationId);

            return _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = CorrelationId,
                ["Action"] = actionName
            })!;
        }

        protected ActionResult HandleResult<TResult>(OperationResult<TResult> result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException(
                    "HandleResult should only be called for failed results.");

            var error = result.Errors?.FirstOrDefault()
                        ?? new Error(
                            ErrorCode.UnknownError,
                            message: "An unknown error occurred.",
                            details: "No error details provided.",
                            correlationId: CorrelationId);

            return CreateErrorResponse(error, result.Timestamp);
        }

        private ActionResult CreateErrorResponse(Error error, DateTime? timestamp = null)
        {
            var response = new ErrorResponse
            {
                Timestamp = timestamp ?? DateTime.UtcNow,
                CorrelationId = error.CorrelationId ?? CorrelationId,
                Errors = new List<string> { error.Message },
                ErrorsDetails = string.IsNullOrWhiteSpace(error.Details)
                    ? null
                    : new List<string> { error.Details },
                ErrorCodes = new List<string> { error.Code.ToString() },
                StatusCode = error.HttpStatus,
                StatusPhrase = error.Severity,
                Path = HttpContext.Request.Path,
                Method = HttpContext.Request.Method,
                Detail = $"Request failed: {error.Message}"
            };

            // Log only once, structured, safe
            _logger.LogWarning(
                "Request failed. Code: {ErrorCode}, CorrelationId: {CorrelationId}, Path: {Path}",
                error.Code,
                response.CorrelationId,
                response.Path);

            return StatusCode(error.HttpStatus, response);
        }
    }
}
