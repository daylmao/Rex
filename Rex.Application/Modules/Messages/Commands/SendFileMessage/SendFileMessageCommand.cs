using Microsoft.AspNetCore.Http;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Messages.Commands.SendFileMessage;

public record SendFileMessageCommand(
    Guid ChatId,
    Guid UserId,
    string? Message,
    List<IFormFile> Files
) : ICommand<MessageDto>;