using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Configs;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.User.Commands.ResendCode;

public class ResendCodeCommandHandler(
    ILogger<ResendCodeCommandHandler> logger,
    IUserRepository userRepository,
    IEmailService emailService,
    ICodeService codeService
) : ICommandHandler<ResendCodeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to resend code.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! We didn't get any information to resend the code."));
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found when trying to resend code.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "Hmm, we couldn't find an account with that email."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(user.Id, cancellationToken);
        if (accountConfirmed)
        {
            logger.LogInformation("Account already confirmed. No need to resend code.");
            return ResultT<ResponseDto>.Failure(Error.Conflict("409",
                "Your account is already confirmed. No need to resend the code."));
        }

        var code = await codeService.CreateCodeAsync(user.Id, CodeType.ConfirmAccount, cancellationToken);
        if (!code.IsSuccess)
        {
            logger.LogWarning("Failed to create confirmation code.");
            return ResultT<ResponseDto>.Failure(code.Error!);
        }

        await emailService.SendEmailAsync(new EmailDto(
            request.Email,
            EmailTemplate.ConfirmAccountTemplate(user.FirstName, user.LastName, code.Value),
            "Confirm Your Account"
        ));

        logger.LogInformation("Confirmation email sent.");
        return ResultT<ResponseDto>.Success(
            new ResponseDto("Great! We've sent a new confirmation code to your email."));
    }
}