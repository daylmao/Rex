using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Friendship;
using Rex.Application.Pagination;

namespace Rex.Application.Modules.Friendships.Queries.GetFriendshipsRequest;

public record GetFriendshipsRequestQuery(
    Guid UserId,
    int PageNumber,
    int PageSize
    ): IQuery<PagedResult<FriendshipRequestDto>>;