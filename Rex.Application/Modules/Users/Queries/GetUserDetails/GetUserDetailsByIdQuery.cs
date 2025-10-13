using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.User;

namespace Rex.Application.Modules.User.Queries.GetUserDetails;

public record GetUserDetailsByIdQuery(
    Guid UserId
    ): IQuery<UserProfileDto>;