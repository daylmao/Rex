using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
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
    ): ICommandHandler<ResendCodeCommand, string>
{
    public async Task<ResultT<string>> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Resend code attempt with null request.");
            return ResultT<string>.Failure(Error.Failure("400", "Request cannot be null."));
        }
    
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Resend code attempt failed: user not found with email {Email}.", request.Email);
            return ResultT<string>.Failure(Error.NotFound("404", "User not found."));
        }
    
        var accountConfirmed = await userRepository.ConfirmedAccountAsync(user.Id, cancellationToken);
        if (accountConfirmed)
        {
            logger.LogInformation("Resend code attempt aborted: account already confirmed for user {UserId}.", user.Id);
            return ResultT<string>.Failure(Error.Conflict("409", "Account already confirmed."));
        }

        var code = await codeService.CreateCodeAsync(user.Id, CodeType.ConfirmAccount, cancellationToken);
        if (!code.IsSuccess)
        {
            logger.LogWarning("Failed to create confirmation code for user {UserId}. Error: {Error}", user.Id, code.Error);
            return ResultT<string>.Failure(code.Error!);
        }
    
        await emailService.SendEmailAsync(new EmailDto( 
            request.Email, 
            EmailTemplate.ConfirmAccountTemplate(user.FirstName, user.LastName, code.Value), 
            "Confirm Account" ));
    
        logger.LogInformation("Confirmation email sent to {Email}.", request.Email);

        return ResultT<string>.Success("Confirmation code sent successfully.");
    }

}