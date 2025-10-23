using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Modules.User.Commands.RegisterUser;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    ILogger<RegisterUserCommandHandler> logger,
    IUserRepository userRepository,
    IUserRoleRepository roleRepository,
    ICloudinaryService cloudinaryService
) : ICommandHandler<RegisterUserCommand, RegisterUserDto>
{
    public async Task<ResultT<RegisterUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.EmailExistAsync(request.Email, cancellationToken);
        if (userExists)
        {
            logger.LogInformation("Email already exists.");
            return ResultT<RegisterUserDto>.Failure(Error.Conflict("409",
                "Looks like this email is already in use. Try logging in or use another email."));
        }

        var userNameExists = await userRepository.UserNameExistAsync(request.UserName, cancellationToken);
        if (userNameExists)
        {
            logger.LogInformation("Username already exists.");
            return ResultT<RegisterUserDto>.Failure(Error.Conflict("409",
                "Oops! This username is already taken. Try another one."));
        }

        var roleExists = await roleRepository.GetRoleByNameAsync(UserRole.User.ToString(), cancellationToken);
        if (roleExists is null)
        {
            logger.LogWarning("Role does not exist.");
            return ResultT<RegisterUserDto>.Failure(Error.Failure("400",
                "Something went wrong: the user role does not exist."));
        }

        string profilePhotoUrl = "";
        if (request.ProfilePhoto is not null)
        {
            await using var stream = request.ProfilePhoto.OpenReadStream();
            profilePhotoUrl =
                await cloudinaryService.UploadImageAsync(stream, request.ProfilePhoto.FileName, cancellationToken);
            logger.LogInformation("Profile photo uploaded.");
        }

        string coverPhotoUrl = "";
        if (request.CoverPhoto is not null)
        {
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverPhotoUrl =
                await cloudinaryService.UploadImageAsync(stream, request.CoverPhoto.FileName, cancellationToken);
            logger.LogInformation("Cover photo uploaded.");
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
            Gender = request.Gender.ToString(),
            Birthday = request.Birthday,
            Status = UserStatus.Active.ToString(),
            RoleId = roleExists.Id,
            ConfirmedAccount = false,
        };

        await userRepository.CreateAsync(user, cancellationToken);
        logger.LogInformation("User created successfully in database.");
        
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
            Birthday: user.Birthday.Value
        );

        return ResultT<RegisterUserDto>.Success(userDto);
    }
}