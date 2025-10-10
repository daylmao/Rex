using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Group;
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
            logger.LogWarning("GetGroupsPaginatedQuery: Request is null.");
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("400", "Oops! No data provided to fetch groups."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("GetGroupsPaginatedQuery: User with ID {UserId} not found.", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Failure(
                Error.Failure("404", "We couldn't find your account."));
        }

        var groups = await distributedCache.GetOrCreateAsync(
            $"groups:available:user:{request.UserId}:page:{request.PageNumber}:size:{request.PageSize}",
            async () => await groupRepository.GetGroupsPaginatedAsync(
                request.UserId,
                page: request.PageNumber,
                size: request.PageSize,
                cancellationToken),
            logger: logger,
            cancellationToken: cancellationToken
        );

        if (!groups.Items.Any())
        {
            logger.LogInformation("No available groups to join for user {UserId}", request.UserId);
            return ResultT<PagedResult<GroupDetailsDto>>.Success(
                new PagedResult<GroupDetailsDto>([], groups.TotalItems, groups.ActualPage, groups.TotalPages)
            );
        }

        var elements = groups.Items
            .Select(g => new GroupDetailsDto(
                g.Id,
                g.ProfilePhoto,
                g.CoverPhoto ?? string.Empty,
                g.Title,
                g.Description,
                g.Visibility,
                g.UserGroups.Count,
                false  
            ))
            .ToList();

        var result = new PagedResult<GroupDetailsDto>(
            items: elements,
            totalItems: groups.TotalItems,
            actualPage: groups.ActualPage,
            pageSize: groups.TotalPages
        );

        logger.LogInformation("Successfully retrieved {Count} available groups for user {UserId}", 
            elements.Count, request.UserId);

        return ResultT<PagedResult<GroupDetailsDto>>.Success(result);
    }
}