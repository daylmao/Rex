using Rex.Enum;

namespace Rex.Application.DTOs;

public record GetGroupMembersRequestDto(string? SearchTerm,
    GroupRole? RoleFilter);