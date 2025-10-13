using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Configs;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Users.Commands.UpdatePasswordByEmail;

public class UpdatePasswordByEmailCommandHandler(
    ILogger<UpdatePasswordByEmailCommandHandler> logger,
    IUserRepository userRepository,
    IEmailService emailService,
    ICodeService codeService
    ): ICommandHandler<UpdatePasswordByEmailCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdatePasswordByEmailCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdatePasswordByEmailCommand received as null");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "We couldn’t process your request. Please try again."));
        }

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"Password reset requested for non-existing email: {request.Email}");
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "We couldn’t find an account with this email address."));
        }

        var code = await codeService.CreateCodeAsync(user.Id, CodeType.ForgotPassword, cancellationToken);
        if (!code.IsSuccess)
        {
            logger.LogWarning($"Could not generate password reset code for user ID {user.Id}: {code.Error}");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("422", "We couldn’t generate a reset code at this time. Please try again."));
        }

        await emailService.SendEmailAsync(new EmailDto(
            request.Email,
            EmailTemplate.ConfirmForgotPasswordTemplate(user.FirstName, user.LastName, code.Value),
            "Your Rex Password Reset Code")
        );

        logger.LogInformation($"Password reset code sent to {user.Email}");
        return ResultT<ResponseDto>.Success(new("We’ve sent a verification code to your email. Enter it in the app to reset your password."));
    }

}