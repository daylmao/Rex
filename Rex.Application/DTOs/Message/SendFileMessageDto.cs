using Microsoft.AspNetCore.Http;

namespace Rex.Application.DTOs.Message;

public record SendFileMessageDto(
    Guid ChatId,
    string? Message,
    List<IFormFile> Files
    );