using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Business.Services;

public class ChatService(MessageService messageService)
{
    public Guid GetAuthenticatedUserId(HubCallerContext context)
    {
        var idClaim =
            context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User?.FindFirst("sub")?.Value;

        if (idClaim == null || !Guid.TryParse(idClaim, out var userId))
            throw new HubException("User not authenticated");

        return userId;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, MessageDto dto)
    {
        dto.SenderId = senderId;
        return await messageService.SendMessageAsync(dto);
    }

    public async Task<IEnumerable<MessageDto>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        if (userId == otherUserId)
            return [];

        return await messageService.GetConversationIfParticipantAsync(userId, otherUserId) ?? [];
    }
}
