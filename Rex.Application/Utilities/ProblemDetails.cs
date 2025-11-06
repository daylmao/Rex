namespace Rex.Application.Utilities;

public record ProblemDetails(
    bool HasError, 
    string? Message = null, 
    int? Code = null, 
    string? Details = null, 
    Dictionary<string, string[]>? Errors = null)
{
    public static ProblemDetails Fail(string message, int code, string? details = null, Dictionary<string, string[]>? errors = null)
        => new ProblemDetails(true, message, code, details, errors);
};