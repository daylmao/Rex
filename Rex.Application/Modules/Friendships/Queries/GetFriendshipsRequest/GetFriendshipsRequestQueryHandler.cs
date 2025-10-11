using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Friendship;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Friendships.Queries.GetFriendshipsRequest;

public class GetFriendshipsRequestQueryHandler(
    ILogger<GetFriendshipsRequestQueryHandler> logger,
    IFriendShipRepository friendShipRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetFriendshipsRequestQuery, PagedResult<FriendshipRequestDto>>
{
    public async Task<ResultT<PagedResult<FriendshipRequestDto>>> Handle(GetFriendshipsRequestQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetFriendshipsRequestQuery is null");
            return ResultT<PagedResult<FriendshipRequestDto>>.Failure(
                Error.Failure("400", "Oops! Something went wrong with your request. Please try again."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", request.UserId);
            return ResultT<PagedResult<FriendshipRequestDto>>.Failure(
                Error.NotFound("404", "We couldn't find your account. Please log in again."));
        }

        var friendship = await cache.GetOrCreateAsync(
            $"get:friendships:by:{request.UserId}:{request.PageNumber}:{request.PageSize}",
            async () => await friendShipRepository.GetFriendShipRequestsByUserIdAsync(request.UserId,
                request.PageNumber, request.PageSize, RequestStatus.Pending,
                cancellationToken),
            logger,
            cancellationToken: cancellationToken
        );

        if (!friendship.Items.Any())
        {
            logger.LogInformation("No pending friend requests found for user {UserId}", request.UserId);
            return ResultT<PagedResult<FriendshipRequestDto>>.Success(
                new PagedResult<FriendshipRequestDto>([], friendship.TotalItems, friendship.ActualPage,
                    friendship.TotalPages));
        }

        var elements = friendship.Items.Select(c => new FriendshipRequestDto(
                c.Id,
                c.Requester.Id,
                $"{c.Requester.FirstName} {c.Requester.LastName}",
                c.Status.ToString(),
                c.Requester.ProfilePhoto,
                c.CreatedAt
            ))
            .ToList();

        var result = new PagedResult<FriendshipRequestDto>(
            items: elements,
            totalItems: friendship.TotalItems,
            actualPage: friendship.ActualPage,
            pageSize: friendship.TotalPages
        );
        
        logger.LogInformation("Successfully retrieved {Count} friend requests for user {UserId}", 
            elements.Count, request.UserId);
        
        return ResultT<PagedResult<FriendshipRequestDto>>.Success(result);
    }
}