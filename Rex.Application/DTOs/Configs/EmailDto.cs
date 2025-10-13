namespace Rex.Application.DTOs.Configs;

public record EmailDto(
    string User,
    string Body,
    string Subject
    );