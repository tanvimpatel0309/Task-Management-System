namespace TaskManagement.DTO.Utility;

public sealed class ApiErrorResponse
{
    public ApiErrorResponse(string message, IDictionary<string, string[]>? errors = null)
    {
        Message = message;
        Errors = errors;
    }

    public string Message { get; }
    public IDictionary<string, string[]>? Errors { get; }
}