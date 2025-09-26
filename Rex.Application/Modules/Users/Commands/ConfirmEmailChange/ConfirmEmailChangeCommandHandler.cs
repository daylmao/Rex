using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    ILogger<ConfirmEmailChangeCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
) : ICommandHandler<ConfirmEmailChangeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("ConfirmEmailChangeCommand request is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request cannot be null"));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found for email confirmation", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "User not found"));
        }

        var codeResult = await codeService.ConfirmCodeAsync(request.UserId, request.Code, cancellationToken);
        if (!codeResult.IsSuccess)
        {
            logger.LogWarning("Failed to confirm email for user {UserId}: {Error}", request.UserId, codeResult.Error.Description);
            return ResultT<ResponseDto>.Failure(codeResult.Error ?? Error.Failure("400", "Unknown error"));
        }

        logger.LogInformation("User {UserId} successfully confirmed email", request.UserId);
        return ResultT<ResponseDto>.Success(new ResponseDto("Email confirmed successfully"));
    }
}
