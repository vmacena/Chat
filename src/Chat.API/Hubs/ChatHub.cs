using System;
using System.Threading.Tasks;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ChatService _chatService;

    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessage(SendMessageRequest dto)
    {
        var senderId = _chatService.GetAuthenticatedUserId(Context);
        var message = new MessageDto
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content,
            SentAt = DateTime.UtcNow,
            Id = Guid.NewGuid(),
        };

        var sentMessage = await _chatService.SendMessageAsync(senderId, message);

        await Clients
            .User(sentMessage.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", sentMessage);
        await Clients
            .User(sentMessage.SenderId.ToString())
            .SendAsync("ReceiveMessage", sentMessage);
    }

    public async Task GetConversation(string otherId)
    {
        var userId = _chatService.GetAuthenticatedUserId(Context);

        if (!Guid.TryParse(otherId, out var otherUserId))
        {
            await Clients.Caller.SendAsync("ReceiveConversation", Array.Empty<MessageDto>());
            return;
        }

        var conversation = await _chatService.GetConversationAsync(userId, otherUserId);
        await Clients.Caller.SendAsync("ReceiveConversation", conversation);
    }
}
