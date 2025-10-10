using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;
using File = Rex.Models.File;

namespace Rex.Application.Modules.Challenges.CreateChallenge;

public class CreateChallengeCommandHandler(
    ILogger<CreateChallengeCommandHandler> logger,
    IChallengeRepository challengeRepository,
    IEntityFileRepository entityFileRepository,
    IFileRepository fileRepository,
    IUserRepository userRepository,
    ICloudinaryService cloudinaryService,
    IGroupRepository groupRepository
) : ICommandHandler<CreateChallengeCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request for creating a challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! We couldn't process your request. Please provide the challenge details."));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with the provided GroupId.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "We couldn't find the group you selected."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("No user found with the provided UserId.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "We couldn't find your account. Please check your details."));
        }

        string coverPhotoUrl = "";
        if (request.CoverPhoto is not null)
        {
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverPhotoUrl =
                await cloudinaryService.UploadImageAsync(stream, request.CoverPhoto.FileName, cancellationToken);
            logger.LogInformation("Cover photo uploaded.");
        }

        Challenge challenge = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Status = ChallengeStatus.Active.ToString(),
            CoverPhoto = coverPhotoUrl,
            CreatorId = request.UserId,
            GroupId = request.GroupId
        };

        await challengeRepository.CreateAsync(challenge, cancellationToken);
        logger.LogInformation("Challenge '{ChallengeTitle}' saved successfully.", request.Title);

        string coverPhoto = string.Empty;
        if (request.CoverPhoto != null)
        {
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverPhoto = await cloudinaryService.UploadImageAsync(
                stream,
                request.CoverPhoto.FileName,
                cancellationToken
            );
            logger.LogInformation("Cover photo uploaded successfully for challenge '{ChallengeTitle}'.", request.Title);
        }

        File file = new()
        {
            Id = Guid.NewGuid(),
            Url = coverPhoto,
            Type = FileType.Image.ToString(),
            UploadedAt = DateTime.UtcNow
        };

        await fileRepository.CreateAsync(file, cancellationToken);
        logger.LogInformation("File registered successfully for challenge '{ChallengeTitle}'.", request.Title);

        EntityFile entityFile = new()
        {
            Id = Guid.NewGuid(),
            TargetId = challenge.Id,
            FileId = file.Id,
            TargetType = TargetType.Challenge.ToString()
        };

        await entityFileRepository.CreateAsync(entityFile, cancellationToken);
        logger.LogInformation("Cover photo linked successfully to challenge '{ChallengeTitle}'.", request.Title);

        logger.LogInformation("Challenge '{ChallengeTitle}' created successfully.", request.Title);
        return ResultT<ResponseDto>.Success(new ResponseDto("Your challenge has been created successfully!"));
    }
}