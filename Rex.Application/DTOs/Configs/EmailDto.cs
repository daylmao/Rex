namespace Rex.Application.DTOs;

public record EmailDto(
    string User,
    string Body,
    string Subject
    );