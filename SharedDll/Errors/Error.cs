using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDll.Errors
{
    public sealed class Error : IEquatable<Error>
    {
        public ErrorCode Code { get; }
        public string Message { get; }
        public string? Details { get; }
        public string? CorrelationId { get; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string Severity { get; }
        public int HttpStatus { get; }

        public Error(
            ErrorCode code,
            string? message = null,
            string? details = null,
            string? correlationId = null)
        {
            Code = code;
            Message = message ?? code.GetDescription();
            Details = details;
            CorrelationId = correlationId;
            Severity = code.ToSeverity().ToString();
            HttpStatus = (int)code.ToHttpStatusCode();
        }

        public static Error FromException(ErrorCode code, Exception ex, string operation, string? correlationId = null) =>
            new Error(code, details: $"Error during {operation}. Details: {ex.Message}. Source: {ex.Source}", correlationId: correlationId);

        public override string ToString() =>
           string.IsNullOrEmpty(Details)
               ? $"[{Timestamp:O}] ({Severity}) Error {Code} ({HttpStatus}): {Message}"
               : $"[{Timestamp:O}] ({Severity}) Error {Code} ({HttpStatus}): {Message} - Details: {Details}";

        public bool Equals(Error? other)
        {
            if (other is null) return false;
            return Code == other.Code && Message == other.Message && Details == other.Details && HttpStatus == other.HttpStatus;
        }

        public override bool Equals(object? obj) => Equals(obj as Error);
        public override int GetHashCode() => HashCode.Combine(Code, Message, Details, HttpStatus);
    }
}
