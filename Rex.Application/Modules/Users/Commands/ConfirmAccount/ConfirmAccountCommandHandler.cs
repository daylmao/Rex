using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler(
    ILogger<ConfirmAccountCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
) : ICommandHandler<ConfirmAccountCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogInformation("Received empty confirm account request.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! No information was provided to confirm your account."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogInformation("User not found for account confirmation.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your account."));
        }

        var confirmUser = await codeService.ConfirmCodeAsync(user.Id, request.Code, cancellationToken);
        if (!confirmUser.IsSuccess)
        {
            logger.LogInformation("Failed to confirm account: {Error}", confirmUser.Error.Description);
            return ResultT<ResponseDto>.Failure(confirmUser.Error);
        }

        logger.LogInformation("Account confirmed successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Great! Your account has been confirmed successfully."));
    }
}