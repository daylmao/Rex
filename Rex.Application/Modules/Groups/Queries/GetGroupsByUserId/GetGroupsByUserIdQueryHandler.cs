using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Queries.GetGroupsByUserId;

public class GetGroupsByUserIdQueryHandler(
    ILogger<GetGroupsByUserIdQueryHandler> logger,
    IGroupRepository groupRepository,
    IDistributedCache distributedCache,
    IUserRepository userRepository
) : IQueryHandler<GetGroupsByUserIdQuery, PagedResult<GroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> Handle(GetGroupsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("GetGroupsByUserIdQuery: Request is null.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "Oops! We didn't receive any data to fetch groups."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("GetGroupsByUserIdQuery: User with ID {UserId} not found.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "We couldn't find your user account."));
        }

        var result = await distributedCache.GetOrCreateAsync(
            $"groups:user:{request.UserId}:page:{request.pageNumber}:size:{request.pageSize}",
            async () => await groupRepository.GetGroupsByUserIdAsync(
                request.UserId,
                request.pageNumber,
                request.pageSize,
                cancellationToken
            ),
            logger,
            cancellationToken: cancellationToken
        );

        if (!result.Items.Any())
        {
            logger.LogInformation("No groups found for user {UserId}", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Success(
                new PagedResult<GroupDetailsDto>([], result.TotalItems, result.ActualPage, result.TotalPages)
            );
        }

        var elements = result.Items
            .Select(c => new GroupDetailsDto(
                ProfilePicture: c.ProfilePhoto,
                CoverPicture: c.CoverPhoto ?? string.Empty,
                Title: c.Title,
                Description: c.Description,
                Visibility: c.Visibility,
                MemberCount: c.UserGroups.Count,
                IsJoined: c.UserGroups.Any(ug => ug.UserId == request.UserId)
            ))
            .ToList();

        var pagedResult = new PagedResult<GroupDetailsDto>(
            elements,
            result.TotalItems,
            result.ActualPage,
            result.TotalPages
        );

        logger.LogInformation("Successfully retrieved {Count} groups for user {UserId}", 
            elements.Count, request.UserId);

        return ResultT<PagedResult<GroupDetailsDto>>.Success(pagedResult);
    }
}