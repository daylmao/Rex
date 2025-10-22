namespace Rex.Application.DTOs.JWT;

public record GithubResponseDto(
    string AccessToken,
    string RefreshToken,
    Guid UserId
    );