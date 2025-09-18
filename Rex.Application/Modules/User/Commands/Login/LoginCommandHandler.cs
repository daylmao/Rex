using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.User.Commands.Login;

public class LoginCommandHandler(
    ILogger<LoginCommandHandler> logger,
    IUserRepository userRepository,
    IAuthenticationService authenticationService
    ): ICommandHandler<LoginCommand, TokenAnswerDto>
{
    public async Task<ResultT<TokenAnswerDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Login attempt with null request.");
            return ResultT<TokenAnswerDto>.Failure(Error.Failure("400", "Request cannot be null."));
        }
    
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Login attempt failed: user not found with email {Email}.", request.Email);
            return ResultT<TokenAnswerDto>.Failure(Error.Failure("404", "User not found."));
        }

        if (user.Status == UserStatus.Banned.ToString())
        {
            logger.LogWarning("Login attempt failed: user {UserId} is banned.", user.Id);
            return ResultT<TokenAnswerDto>.Failure(Error.Failure("403", "User is banned."));
        }

        // var confirmedAccount = await userRepository.ConfirmedAccountAsync(user.Id, cancellationToken);
        // if (!confirmedAccount)
        // {
        //     logger.LogWarning("Login attempt failed: account not confirmed for user {UserId}.", user.Id);
        //     return ResultT<TokenAnswerDto>.Failure(Error.Failure("403", "Account not confirmed."));
        // }
    
        var verifiedPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!verifiedPassword)
        {
            logger.LogWarning("Login attempt failed: invalid password for user {UserId}.", user.Id);
            return ResultT<TokenAnswerDto>.Failure(Error.Failure("401", "Invalid password."));
        }
            
        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);
        var accessToken = await authenticationService.GenerateTokenAsync(user, cancellationToken);
        var refreshToken = await authenticationService.GenerateRefreshTokenAsync(user, cancellationToken);
    
        logger.LogInformation("User {UserId} logged in successfully.", user.Id);
    
        return ResultT<TokenAnswerDto>.Success(new TokenAnswerDto(accessToken, refreshToken));
    }

}