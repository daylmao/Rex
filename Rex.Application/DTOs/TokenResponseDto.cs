namespace Rex.Application.DTOs;

public record TokenResponseDto(
    string AccessToken,
    string RefreshToken
    );