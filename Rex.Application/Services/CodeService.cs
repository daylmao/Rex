using Microsoft.Extensions.Logging;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Services;

public class CodeService(
    ICodeRepository codeRepository,
    IUserRepository userRepository,
    ILogger<CodeService> logger
    ): ICodeService
{
    public async Task<ResultT<string>> CreateCodeAsync(Guid userId, CodeType codeType, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning($"User with ID {userId} not found");
            return ResultT<string>.Failure(Error.NotFound("404", "User not found"));
        }

        if (codeType == CodeType.ConfirmAccount && user.ConfirmedAccount)
        {
            logger.LogWarning($"User with ID {userId} already has a confirmed account");
            return ResultT<string>.Failure(Error.Failure("409", "Account already confirmed"));
        }
    
        string generatedCode = CodeGenerator.GenerateCode();

        Code code = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Value = generatedCode,
            Expiration = DateTime.UtcNow.AddMinutes(15),
            Type = codeType.ToString()
        };
    
        await codeRepository.CreateCodeAsync(code, cancellationToken);
        logger.LogInformation($"Code {generatedCode} created for user ID {userId} with type {codeType}");

        return ResultT<string>.Success(code.Value);
    }


    public async Task<ResultT<CodeDto>> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var code = await codeRepository.GetCodeByIdAsync(id, cancellationToken);

        if (code is null)
        {
            logger.LogWarning("Code with the given ID does not exist");
            return ResultT<CodeDto>.Failure(Error.NotFound("404", "Code with the specified ID does not exist"));
        }

        CodeDto codeDto = new
        (
            CodeId: code.Id,
            UserId: code.UserId,
            Code: code.Value,
            IsUsed: code.Used,
            Expiration: code.Expiration
        );
    
        logger.LogInformation($"Code with ID {id} retrieved successfully");
        return ResultT<CodeDto>.Success(codeDto);
    }


    public async Task<Result> ConfirmCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User with ID {userId} not found");
            return ResultT<string>.Failure(Error.NotFound("404", "User not found"));
        }
    
        var codeEntity = await codeRepository.GetCodeByValueAsync(code, cancellationToken);
        if (codeEntity is null)
        {
            logger.LogWarning($"Code '{code}' does not exist");
            return Result.Failure(Error.NotFound("404", "Code with the specified value does not exist"));
        }

        if (codeEntity.UserId != userId)
        {
            logger.LogWarning($"Code '{code}' does not belong to user ID {userId}");
            return Result.Failure(Error.Failure("403", "Code does not belong to the specified user"));
        }

        if (codeEntity.Used)
        {
            logger.LogWarning($"Code '{code}' has already been used");
            return Result.Failure(Error.Failure("400", "Code has already been used"));
        }
    
        var isValidCode = await codeRepository.IsCodeValidAsync(code, cancellationToken);
        if (!isValidCode)
        {
            logger.LogWarning($"Code '{code}' is expired or invalid");
            return Result.Failure(Error.Failure("400", "Code is invalid or expired"));
        }

        if (codeEntity.Type == CodeType.EmailConfirmation.ToString())
        {
            user.ConfirmedAccount = true;
            await userRepository.UpdateAsync(user, cancellationToken);
            
            await codeRepository.MarkCodeAsUsedAsync(code, cancellationToken);
            logger.LogInformation("User {UserId} successfully changed email", user.Id);
            
            return Result.Success();
        }

        user.ConfirmedAccount = true;
        await userRepository.UpdateAsync(user, cancellationToken);
        
        await codeRepository.MarkCodeAsUsedAsync(code, cancellationToken);
        
        logger.LogInformation("User {UserId} account confirmed successfully", user.Id);
        return Result.Success();
    }


    public async Task<Result> IsCodeValidAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code))
        {
            logger.LogWarning("The provided code is null or empty");
            return ResultT<string>.Failure(Error.Failure("400", "The code cannot be empty"));
        }
    
        var isUsed = await codeRepository.IsCodeUsedAsync(code, cancellationToken);

        if (isUsed)
        {
            logger.LogWarning("The code has already been used");
            return ResultT<string>.Failure(Error.Conflict("409", "This code has already been used"));
        }
    
        logger.LogInformation("The code is valid and available");
        return Result.Success();
    }


    public async Task<ResultT<string>> ValidateCodeAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(code))
        {
            logger.LogWarning("The code provided is null or empty");
            return ResultT<string>.Failure(Error.Failure("400", "The code cannot be empty"));
        }
    
        var isValid = await codeRepository.IsCodeValidAsync(code, cancellationToken);

        if (!isValid)
        {
            logger.LogWarning("The code is either used or expired");
            return ResultT<string>.Failure(Error.Conflict("409", "The code is invalid, either used or expired"));
        }
    
        logger.LogInformation("The code is valid and can be used");
        return ResultT<string>.Success("The code is valid");
    }

}