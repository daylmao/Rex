using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.User.Commands.UpdateEmail;

public class UpdateEmailCommandHandler(
    ILogger<UpdateEmailCommandHandler> logger,
    IUserRepository userRepository,
    IEmailService emailService,
    ICodeService codeService
) : ICommandHandler<UpdateEmailCommand, AnswerDto>
{
    public async Task<ResultT<AnswerDto>> Handle(UpdateEmailCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateEmailCommand request is null");
            return ResultT<AnswerDto>.Failure(Error.Failure("400", "Request cannot be null"));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found for email update", request.UserId);
            return ResultT<AnswerDto>.Failure(Error.NotFound("404", "User not found"));
        }

        var isEmailInUseByYou = await userRepository.EmailInUseByYouAsync(request.UserId, request.Email, cancellationToken);
        if (!isEmailInUseByYou)
        {
            logger.LogWarning("User {UserId} attempted to change email from {Email}, but it's not their current email", request.UserId, request.Email);
            return ResultT<AnswerDto>.Failure(Error.Failure("400", "Current email mismatch"));
        }

        var emailExists = await userRepository.EmailExistAsync(request.NewEmail, cancellationToken);
        if (emailExists)
        {
            logger.LogWarning("New email {NewEmail} for user {UserId} is already in use", request.NewEmail, request.UserId);
            return ResultT<AnswerDto>.Failure(Error.Conflict("409", "Email already in use"));
        }

        var codeResult = await codeService.CreateCodeAsync(request.UserId, CodeType.EmailConfirmation, cancellationToken);
        if (!codeResult.IsSuccess)
        {
            logger.LogWarning("Failed to create confirmation code for user {UserId}", request.UserId);
            return ResultT<AnswerDto>.Failure(codeResult.Error ?? Error.Failure("400", "Could not create code"));
        }

        user.Email = request.NewEmail;
        user.ConfirmedAccount = false;
        await emailService.SendEmailAsync(new EmailDto(
            user.Email,
            EmailTemplate.ConfirmEmailChangeTemplate(user.FirstName, user.Email, request.NewEmail, codeResult.Value),
            "Email Change Confirmation"
        ));

        await userRepository.UpdateAsync(user, cancellationToken);
        
        logger.LogInformation("Confirmation email sent for user {UserId} to {NewEmail}", request.UserId, request.NewEmail);
        return ResultT<AnswerDto>.Success(new AnswerDto("Confirmation email sent successfully"));
    }

}