namespace Rex.Application.DTOs.JWT;

public record TokenResponseDto(
    string AccessToken,
    string RefreshToken
    );