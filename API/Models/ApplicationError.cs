namespace API.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string TraceId { get; init; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; init; }

        public ErrorResponse(int statusCode, string message, string type, string traceId)
        {
            StatusCode = statusCode;
            Message = message;
            Type = type;
            TraceId = traceId;
        }
    }
}
