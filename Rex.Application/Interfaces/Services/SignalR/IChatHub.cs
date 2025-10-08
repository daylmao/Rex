using Rex.Application.DTOs;

namespace Rex.Application.Interfaces.SignalR;

public interface IChatHub
{
    Task SendMessageResult(SignalResponse response);
    Task ReceiveMessage(MessageDto message);
    Task ReceiveChatCreated(ChatDto chat);
}