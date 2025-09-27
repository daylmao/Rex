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
) : ICommandHandler<LoginCommand, TokenResponseDto>
{
    public async Task<ResultT<TokenResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty login request.");
            return ResultT<TokenResponseDto>.Failure(Error.Failure("400",
                "Oops! Looks like you didn't enter any login info."));
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", request.Email);
            return ResultT<TokenResponseDto>.Failure(Error.Failure("404",
                "Hmm, we couldn't find an account with that email."));
        }

        if (user.Status == UserStatus.Banned.ToString())
        {
            logger.LogWarning("User {UserId} is banned.", user.Id);
            return ResultT<TokenResponseDto>.Failure(Error.Failure("403",
                "Your account has been blocked. Please reach out to support for help."));
        }

        var verifiedPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!verifiedPassword)
        {
            logger.LogWarning("Invalid password for user {UserId}.", user.Id);
            return ResultT<TokenResponseDto>.Failure(Error.Failure("401",
                "Oops! That password doesn't look right. Try again."));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = await authenticationService.GenerateTokenAsync(user, cancellationToken);
        var refreshToken = await authenticationService.GenerateRefreshTokenAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully.", user.Id);

        return ResultT<TokenResponseDto>.Success(new TokenResponseDto(accessToken, refreshToken));
    }
}