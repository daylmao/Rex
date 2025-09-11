using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.User.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    ILogger<RegisterUserCommandHandler> logger,
    ICodeService codeService,
    IEmailService emailService,
    IUserRepository userRepository,
    ICloudinaryService cloudinaryService
    ): ICommandHandler<RegisterUserCommand, RegisterUserDto>
{
    public async Task<ResultT<RegisterUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.EmailExistAsync(request.Email, cancellationToken);
        if (userExists)
        {
            logger.LogInformation("Email {Email} already exists.", request.Email);
            return ResultT<RegisterUserDto>.Failure(Error.Conflict("409", "The provided email is already registered."));
        }
        
        var userNameExists = await userRepository.UserNameExistAsync(request.UserName, cancellationToken);
        if (userNameExists)
        {
            logger.LogInformation("Username {UserName} already exists.", request.UserName);
            return ResultT<RegisterUserDto>.Failure(Error.Conflict("409", "The provided username is already taken."));
        }

        string profilePhotoUrl = "";
        if (request.ProfilePhoto is not null)
        {
            using var stream = request.ProfilePhoto.OpenReadStream();
            profilePhotoUrl = await cloudinaryService.UploadImageAsync(
                stream,
                request.ProfilePhoto.FileName,
                cancellationToken
            );
            logger.LogInformation("Profile photo uploaded for user {UserName}.", request.UserName);
        }
        
        string coverPhotoUrl = "";
        if (request.CoverPhoto is not null)
        {
            using var stream = request.CoverPhoto.OpenReadStream();
            coverPhotoUrl = await cloudinaryService.UploadImageAsync(
                stream,
                request.CoverPhoto.FileName,
                cancellationToken
            );
            logger.LogInformation("Cover photo uploaded for user {UserName}.", request.UserName);
        }

        Models.User user = new()
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            ProfilePhoto = profilePhotoUrl,
            CoverPhoto = coverPhotoUrl,
            Biography = request.Biography,
            Gender = request.Gender,
            Birthday = request.Birthday,
            Status = UserStatus.Active.ToString(),
            RoleId = request.RolId,
            ConfirmedAccount = false,
        };
        
        await userRepository.CreateAsync(user, cancellationToken);
        logger.LogInformation("User {UserName} created in database with Id {UserId}.", user.UserName, user.Id);
        
        var code = await codeService.CreateCodeAsync(user.Id, CodeType.ConfirmAccount, cancellationToken);

        if (!code.IsSuccess)
        {
            logger.LogError("Failed to create confirmation code for user {UserId}. Error: {Error}", user.Id, code.Value);
            return ResultT<RegisterUserDto>.Failure(Error.Failure("400","Failed to generate confirmation code."));
        }

        await emailService.SendEmailAsync(
            new EmailDto(
                User: request.Email,
                Body: EmailTemplate.ConfirmAccountTemplate(user.FirstName, user.LastName, code.Value),
                Subject: "Confirm Account"
                )
            );
        
        logger.LogInformation("Confirmation email sent to {Email}.", request.Email);
        
        RegisterUserDto userDto = new(
            UserId: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            UserName: user.UserName,
            Email: user.Email,
            ProfilePhoto: user.ProfilePhoto,
            CoverPhoto: user.CoverPhoto,
            Biography: user.Biography,
            Gender: user.Gender,
            Birthday: user.Birthday
        );
        
        logger.LogInformation("User {UserName} registered successfully.", user.UserName);

        return ResultT<RegisterUserDto>.Success(userDto);
    }
}
