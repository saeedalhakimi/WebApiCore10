using Serilog.Context;

namespace WebApiCore10.RustApi.Presentation.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId) || !Guid.TryParse(correlationId, out _))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("Request {Method} {Path} received at {Time}",
                    context.Request.Method, context.Request.Path, DateTime.UtcNow);
                await _next(context);
            }
        }
    }
}
