using Rex.Application.DTOs;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces;

/// <summary>
/// Service for managing user-related codes and their validation.
/// </summary>
public interface ICodeService
{
    /// <summary>
    /// Creates a code for a specific user based on the given code type.
    /// </summary>
    Task<ResultT<string>> CreateCodeAsync(Guid userId, CodeType codeType, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a code by its unique identifier.
    /// </summary>
    Task<ResultT<CodeDto>> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms a user's account using a specific code.
    /// </summary>
    Task<Result> ConfirmAccountAsync(Guid userId, string code, CancellationToken cancellationToken);
    
    /// <summary>
    /// Checks whether the provided code is valid.
    /// </summary>
    Task<Result> IsCodeValidAsync(string code, CancellationToken cancellationToken);
    
    /// <summary>
    /// Validates a code and returns its value if valid.
    /// </summary>
    Task<ResultT<string>> ValidateCodeAsync(string code, CancellationToken cancellationToken);
}