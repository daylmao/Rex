using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    ILogger<ConfirmEmailChangeCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
) : ICommandHandler<ConfirmEmailChangeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ConfirmEmailChangeCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty email confirmation request.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! No data was provided to confirm your email."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found for email confirmation.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "Hmm, we couldn't find an account with that email."));
        }

        var codeResult = await codeService.ConfirmCodeAsync(request.UserId, request.Code, cancellationToken);
        if (!codeResult.IsSuccess)
        {
            logger.LogWarning("Failed to confirm email for user {UserId}: {Error}", request.UserId,
                codeResult.Error.Description);
            return ResultT<ResponseDto>.Failure(codeResult.Error ?? Error.Failure("400",
                "Something went wrong while confirming your email. Please try again."));
        }

        logger.LogInformation("User {UserId} successfully confirmed email.", request.UserId);
        return ResultT<ResponseDto>.Success(new ResponseDto("Great! Your email has been confirmed successfully."));
    }
}