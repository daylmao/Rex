using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Queries.GetUserDetails;

public class GetUserDetailsByIdQueryHandler(
    ILogger<GetUserDetailsByIdQueryHandler> logger,
    IUserRepository userRepository,
    IDistributedCache cache
    ): IQueryHandler<GetUserDetailsByIdQuery, UserProfileDto>
{
    public async Task<ResultT<UserProfileDto>> Handle(GetUserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetUserDetailsByIdQuery request is null.");
            return ResultT<UserProfileDto>.Failure(Error.Failure("400", "Request cannot be null."));
        }
    
        var userCache = await cache.GetOrCreateAsync(
            $"get-user-details-{request.UserId.ToString().ToLower()}",
            async() => await userRepository.GetUserDetailsAsync(request.UserId, cancellationToken), logger ,cancellationToken:cancellationToken);
        
        if (userCache is null)
        {
            logger.LogWarning("GetUserDetailsByIdQuery: user not found for UserId {UserId}", request.UserId);
            return ResultT<UserProfileDto>.Failure(Error.NotFound("404", "User not found"));
        }

        UserProfileDto userDto = new(
            userCache.FirstName,
            userCache.LastName,
            userCache.Email,
            userCache.UserName,
            userCache.ProfilePhoto,
            userCache.CoverPhoto,
            userCache.Birthday,
            userCache.CreatedAt,
            userCache.Biography,
            userCache.Gender,
            userCache.LastLoginAt,
            userCache.UserGroups?.Count ?? 0,
            userCache.Reactions?.Count ?? 0,
            userCache.UserChallenges?.Count ?? 0
        );
    
        logger.LogInformation("User details retrieved successfully for UserId {UserId}.", request.UserId);
        return ResultT<UserProfileDto>.Success(userDto);
    }

}