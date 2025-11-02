namespace Rex.Application.DTOs.JWT;

public record GithubResponseDto(
    string AccessToken,
    Guid UserId
    );