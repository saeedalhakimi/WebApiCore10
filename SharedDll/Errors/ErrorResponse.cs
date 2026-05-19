using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDll.Errors
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? StatusPhrase { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string>? ErrorsDetails { get; set; }
        public List<string>? ErrorCodes { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Detail { get; set; }
        public string? CorrelationId { get; set; }
    }
}
