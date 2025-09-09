namespace Rex.Application.DTOs;

public record TokenAnswerDto(
    string AccessToken,
    string RefreshToken
    );