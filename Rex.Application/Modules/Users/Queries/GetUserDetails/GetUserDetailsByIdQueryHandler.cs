using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Queries.GetUserDetails;

public class GetUserDetailsByIdQueryHandler(
    ILogger<GetUserDetailsByIdQueryHandler> logger,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetUserDetailsByIdQuery, UserProfileDto>
{
    public async Task<ResultT<UserProfileDto>> Handle(GetUserDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request for user details.");
            return ResultT<UserProfileDto>.Failure(Error.Failure("400",
                "Oops! We didn't get the information needed to fetch your profile."));
        }

        var version = await cache.GetVersionAsync("user", request.UserId, cancellationToken);
        var cacheKey = $"get:user:details:{request.UserId.ToString().ToLower()}:version:{version}";
        
        var userCache = await cache.GetOrCreateAsync(
            cacheKey,
            async () => await userRepository.GetUserDetailsAsync(request.UserId, cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (userCache is null)
        {
            logger.LogWarning("User not found when retrieving details.");
            return ResultT<UserProfileDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your profile."));
        }

        UserProfileDto userDto = new(
            userCache.FirstName,
            userCache.LastName,
            userCache.Email,
            userCache.UserName,
            userCache.ProfilePhoto,
            userCache.CoverPhoto,
            userCache.Birthday ?? DateTime.MinValue,
            userCache.CreatedAt,
            userCache.Biography,
            userCache.Gender,
            userCache.LastLoginAt,
            userCache.UserGroups?.Count ?? 0,
            userCache.Reactions?.Count ?? 0,
            userCache.UserChallenges?.Count ?? 0
        );

        logger.LogInformation("User details retrieved successfully.");
        return ResultT<UserProfileDto>.Success(userDto);
    }
}