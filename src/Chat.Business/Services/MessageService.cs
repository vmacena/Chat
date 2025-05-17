using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Business.Exceptions;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Interfaces;

namespace Chat.Business.Services;

public class MessageService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IUserRepository _userRepo;

    public MessageService(IMessageRepository messageRepo, IUserRepository userRepo)
    {
        _messageRepo = messageRepo;
        _userRepo = userRepo;
    }

    public async Task<MessageDto> SendMessageAsync(MessageDto dto)
    {
        if (!await _userRepo.ExistsAsync(dto.SenderId))
            throw new Exception("Remetente não encontrado");

        if (!await _userRepo.ExistsAsync(dto.ReceiverId))
            throw new Exception("Destinatário não encontrado");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Senderid = dto.SenderId,
            Receiverid = dto.ReceiverId,
            Content = dto.Content,
            Sentat = DateTime.UtcNow,
        };

        await _messageRepo.AddAsync(message);

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
            throw new Exception("Conversa inválida");

        var messages = await _messageRepo.GetConversationAsync(userId, otherUserId);

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
