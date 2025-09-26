using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
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
            logger.LogWarning("CreateChallengeCommand was empty. No data was provided to create the challenge.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Invalid request"));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with id {GroupId}.", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Group not found"));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("No user found with id {UserId}.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "User not found"));
        }

        Challenge challenge = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Status = ChallengeStatus.Active.ToString(),
            CreatorId = request.UserId,
            GroupId = request.GroupId
        };

        await challengeRepository.CreateAsync(challenge, cancellationToken);
        logger.LogInformation("Challenge '{ChallengeTitle}' was saved in the repository.", request.Title);
        
        string coverPhoto = string.Empty;
        if (request.CoverPhoto != null)
        {
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverPhoto = await cloudinaryService.UploadImageAsync(
                stream,
                request.CoverPhoto.FileName,
                cancellationToken
            );
            logger.LogInformation("Cover photo for challenge '{ChallengeTitle}' uploaded successfully.", request.Title);
        }
        

        File file = new()
        {
            Id = Guid.NewGuid(),
            Url = coverPhoto,
            Type = FileType.Image.ToString(),
            UploadedAt = DateTime.UtcNow
        };

        await fileRepository.CreateAsync(file, cancellationToken);
        logger.LogInformation("File for challenge '{ChallengeTitle}' was registered successfully.", request.Title);

        EntityFile entityFile = new()
        {
            Id = Guid.NewGuid(),
            TargetId = challenge.Id,
            FileId = file.Id,
            TargetType = TargetType.Challenge.ToString()
        };

        await entityFileRepository.CreateAsync(entityFile, cancellationToken);
        logger.LogInformation("Cover photo was linked to challenge '{ChallengeTitle}' successfully.", request.Title);

        logger.LogInformation("Challenge '{ChallengeTitle}' was created successfully.", request.Title);
        return ResultT<ResponseDto>.Success(new ResponseDto("Challenge created successfully"));
    }
}