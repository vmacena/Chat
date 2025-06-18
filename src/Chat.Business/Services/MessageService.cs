using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Business.Exceptions;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Interfaces;

namespace Chat.Business.Services;

public class MessageService(IMessageRepository messageRepo, IUserRepository userRepo)
{
    public async Task<MessageDto> SendMessageAsync(MessageDto dto)
    {
        if (!await userRepo.ExistsAsync(dto.SenderId))
            throw new Exception("Remetente não encontrado");

        if (!await userRepo.ExistsAsync(dto.ReceiverId))
            throw new Exception("Destinatário não encontrado");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Senderid = dto.SenderId,
            Receiverid = dto.ReceiverId,
            Content = dto.Content,
            Sentat = DateTime.UtcNow,
        };

        await messageRepo.AddAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.Senderid,
            ReceiverId = message.Receiverid,
            Content = message.Content,
            SentAt = message.Sentat,
            SeenAt = message.Seenat,
        };
    }

    public async Task<IEnumerable<MessageDto>> GetConversationIfParticipantAsync(
        Guid userId,
        Guid otherUserId
    )
    {
        if (userId == otherUserId)
            throw new Exception("Invalid Conversation");

        var messages = await messageRepo.GetConversationAsync(userId, otherUserId);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.Senderid,
            ReceiverId = m.Receiverid,
            Content = m.Content,
            SentAt = m.Sentat,
            SeenAt = m.Seenat,
        });
    }
}
