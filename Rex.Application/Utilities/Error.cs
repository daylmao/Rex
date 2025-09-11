using Rex.Enum;

namespace Rex.Application.Utilities;

/// <summary>
/// Represents an error that occurred during an operation, including a code, description, and type.
/// </summary>
public class Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with the specified code, description, and error type.
    /// </summary>
    /// <param name="code">A unique identifier for the error.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <param name="errorType">The type or category of the error.</param>
    private Error(
        string code, 
        string description, 
        ErrorType errorType)
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    /// <summary>
    /// Gets the unique code that identifies the error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the description of the error.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the type or category of the error.
    /// </summary>
    public ErrorType ErrorType { get; }

    /// <summary>
    /// Creates a generic failure error.
    /// </summary>
    /// <param name="code">The code that identifies the error.</param>
    /// <param name="description">A description of the failure.</param>
    /// <returns>An instance of <see cref="Error"/> representing a failure.</returns>
    public static Error Failure(string code, string description) => 
        new Error(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    /// <param name="code">The code that identifies the error.</param>
    /// <param name="description">A description indicating what was not found.</param>
    /// <returns>An instance of <see cref="Error"/> representing a not found error.</returns>
    public static Error NotFound(string code, string description) =>
        new Error(code, description, ErrorType.NotFound);
    
    /// <summary>
    /// Creates a conflict error, typically used when a resource already exists or there is a data conflict.
    /// </summary>
    /// <param name="code">The code that identifies the conflict error.</param>
    /// <param name="description">A description of the conflict.</param>
    /// <returns>An instance of <see cref="Error"/> representing a conflict.</returns>
    public static Error Conflict(string code, string description) =>
        new Error(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="code">The code that identifies the unauthorized error.</param>
    /// <param name="description">A description of the unauthorized access.</param>
    /// <returns>An instance of <see cref="Error"/> representing an unauthorized error.</returns>
    public static Error Unauthorized(string code, string description) =>
        new Error(code, description, ErrorType.Unauthorized);
}
