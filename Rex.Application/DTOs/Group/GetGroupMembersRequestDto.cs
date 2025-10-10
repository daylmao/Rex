using Rex.Enum;

namespace Rex.Application.DTOs.JWT;

public record GetGroupMembersRequestDto(string? SearchTerm,
    GroupRole? RoleFilter);