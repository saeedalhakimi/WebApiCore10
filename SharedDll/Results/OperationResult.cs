using SharedDll.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDll.Results
{
    public class OperationResult<T>
    {
        public T? Data { get; }
        public bool IsError { get; }
        public bool IsSuccess => !IsError;
        public IReadOnlyList<Error> Errors { get; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        private OperationResult(T? data, bool isError, IReadOnlyList<Error> errors)
        {
            Data = data;
            IsError = isError;
            Errors = errors;
        }

        //Success factory methods
        public static OperationResult<T> Success(T payload) => new(payload, false, new List<Error>());
        public static OperationResult<(T1, T2)> Success<T1, T2>(T1 item1, T2 item2) =>
            new((item1, item2), false, new List<Error>());
        public static OperationResult<(bool, byte[]?)> Success(bool isSuccessful, byte[]? data) =>
            new((isSuccessful, data), false, new List<Error>());

        //Error factory methods
        public static OperationResult<T> Failure(Error error) => new(default, true, new List<Error> { error });
        public static OperationResult<T> Failure(IReadOnlyList<Error> errors) => new(default, true, errors.ToList());
        public static OperationResult<T> Failure(ErrorCode code, string message, string? details = null, string? correlationId = null) =>
            new(default, true, new List<Error> { new Error(code, message, details, correlationId) });

        //Utility methods
        public bool HasErrors() => IsError && Errors.Any();
        public string GetErrorMessage() => string.Join("; ", Errors.Select(e => e.Message));
        public string GetFirstErrorMessage() => Errors.FirstOrDefault()?.Message ?? string.Empty;
        public bool HasError(ErrorCode code) => Errors.Any(e => e.Code.Equals(code));

        public override string ToString() =>
           IsError
               ? $"[{Timestamp:O}] Error(s): {GetErrorMessage()}"
               : $"[{Timestamp:O}] Success: {Data}";
    }
}
