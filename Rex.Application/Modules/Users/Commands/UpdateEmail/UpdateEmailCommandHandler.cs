using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Configs;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.User.Commands.UpdateEmail;

public class UpdateEmailCommandHandler(
    ILogger<UpdateEmailCommandHandler> logger,
    IUserRepository userRepository,
    IEmailService emailService,
    ICodeService codeService,
    IDistributedCache cache
) : ICommandHandler<UpdateEmailCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to update email.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! We didn't get the information needed to update your email."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for email update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your account."));
        }

        var isEmailInUseByYou =
            await userRepository.EmailInUseByYouAsync(request.UserId, request.Email, cancellationToken);
        if (!isEmailInUseByYou)
        {
            logger.LogWarning("Current email mismatch.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "The current email you provided doesn't match our records."));
        }

        var emailExists = await userRepository.EmailInUseAsync(request.NewEmail, request.UserId, cancellationToken);
        if (emailExists)
        {
            logger.LogWarning("New email already in use.");
            return ResultT<ResponseDto>.Failure(Error.Conflict("409", "Oops! That new email is already being used."));
        }

        var codeResult =
            await codeService.CreateCodeAsync(request.UserId, CodeType.EmailConfirmation, cancellationToken);
        if (!codeResult.IsSuccess)
        {
            logger.LogWarning("Failed to create confirmation code.");
            return ResultT<ResponseDto>.Failure(codeResult.Error ??
                                                Error.Failure("400", 
                                                    "Something went wrong while generating the confirmation code."));
        }

        user.Email = request.NewEmail;
        user.ConfirmedAccount = false;

        await emailService.SendEmailAsync(new EmailDto(
            user.Email,
            EmailTemplate.ConfirmEmailChangeTemplate(user.FirstName, user.Email, request.NewEmail, codeResult.Value),
            "Confirm Your New Email"
        ));

        await userRepository.UpdateAsync(user, cancellationToken);

        await cache.IncrementVersionAsync("user", request.UserId, logger, cancellationToken);
        
        logger.LogInformation("Confirmation email sent for new email.");
        return ResultT<ResponseDto>.Success(
            new ResponseDto("Great! A confirmation email has been sent to your new address."));
    }
}