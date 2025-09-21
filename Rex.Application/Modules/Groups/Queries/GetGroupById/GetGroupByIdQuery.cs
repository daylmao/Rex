using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;

namespace Rex.Application.Modules.Groups.Queries.GetGroupById;

public record GetGroupByIdQuery(
    Guid GroupId,
    Guid UserId
    ): IQuery<GroupDetailsDto>;