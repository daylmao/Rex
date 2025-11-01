namespace Rex.Application.DTOs.Configs;

public record ProblemDetailsDto(
    bool HasError, 
    string? Message = null, 
    int? Code = null, 
    string? Details = null, 
    Dictionary<string, string[]>? Errors = null)
{
    public static ProblemDetailsDto Fail(string message, int code, string? details = null, Dictionary<string, string[]>? errors = null)
        => new ProblemDetailsDto(true, message, code, details, errors);
};