using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;

public class GetGroupsPaginatedQueryHandler(
    ILogger<GetGroupsPaginatedQueryHandler> logger,
    IGroupRepository groupRepository,
    IDistributedCache distributedCache,
    IUserRepository userRepository
) : IQueryHandler<GetGroupsPaginatedQuery, PagedResult<GroupDetailsDto>>
{
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> Handle(GetGroupsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Request to get paginated groups is null.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "The request cannot be empty."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID: {UserId} not found.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "User not found."));
        }

        var groups = await distributedCache.GetOrCreateAsync(
            $"get-groups-by-userid-{request.UserId}-{request.pageNumber}-{request.pageSize}",
            async () => await groupRepository.GetGroupsPaginatedAsync(
                request.UserId, 
                page: request.pageNumber,
                size: request.pageSize, 
                cancellationToken),
            logger: logger,
            cancellationToken: cancellationToken
        );

        var elements = groups.Items
            .Select(g => new GroupDetailsDto(
                g.ProfilePhoto,
                g.CoverPhoto,
                g.Title,
                g.Description,
                g.Visibility,
                g.UserGroups.Count,
                g.UserGroups.Any(ug => ug.UserId == request.UserId)
            ));

        if (!elements.Any())
        {
            logger.LogWarning("No groups found for user ID: {UserId}", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "No groups found."));
        }

        var result = new PagedResult<GroupDetailsDto>(
            items: elements,
            totalItems: groups.TotalItems,
            actualPage: groups.ActualPage,
            pageSize: request.pageSize
        );

        logger.LogWarning("Successfully retrieved {Count} groups for user ID: {UserId}", elements.Count(),
            request.UserId);
        return ResultT<PagedResult<GroupDetailsDto>>.Success(result);
    }
}