// ChatHub.cs
using System.Security.Claims;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly MessageService _svc;

    public ChatHub(MessageService svc) => _svc = svc;

    public async Task SendMessage(MessageDto dto)
    {
        var idClaim =
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;
        if (idClaim == null || !Guid.TryParse(idClaim, out var sender))
            throw new HubException("Usuário não autenticado");

        dto.SenderId = sender;
        var sent = await _svc.SendMessageAsync(dto);
        await Clients.User(sent.ReceiverId.ToString()).SendAsync("ReceiveMessage", sent);
        await Clients.User(sent.SenderId.ToString()).SendAsync("ReceiveMessage", sent);
    }

    public async Task GetConversation(string otherId)
    {
        var idVal = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (
            idVal == null
            || !Guid.TryParse(idVal, out var me)
            || !Guid.TryParse(otherId, out var other)
        )
        {
            await Clients.Caller.SendAsync("ReceiveConversation", Array.Empty<MessageDto>());
            return;
        }

        var convo =
            await _svc.GetConversationIfParticipantAsync(me, other)
            ?? Enumerable.Empty<MessageDto>();
        await Clients.Caller.SendAsync("ReceiveConversation", convo);
    }
}
