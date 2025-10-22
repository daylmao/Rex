using Rex.Enum;

namespace Rex.Application.DTOs.Group;

public record UpdateGroupRoleMemberDto(Guid GroupId, GroupRole Role);