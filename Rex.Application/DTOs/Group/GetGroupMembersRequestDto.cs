using Rex.Enum;

namespace Rex.Application.DTOs.Group;

public record GetGroupMembersRequestDto(string? SearchTerm,
    GroupRole? RoleFilter);