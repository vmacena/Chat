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
    private readonly IMessageRepository _repo;

    public MessageService(IMessageRepository repo)
    {
        _repo = repo;
    }

    public async Task<MessageDto> SendMessageAsync(MessageDto dto)
    {
        Console.WriteLine(
            $"[SendMessageAsync] Iniciando envio. SenderId: {dto.SenderId}, ReceiverId: {dto.ReceiverId}, Content: {dto.Content}"
        );

        try
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Senderid = dto.SenderId,
                Receiverid = dto.ReceiverId,
                Content = dto.Content,
                Sentat = DateTime.UtcNow,
                Seenat = null,
            };

            Console.WriteLine($"[SendMessageAsync] Mensagem criada. Id: {message.Id}");

            await _repo.AddAsync(message);

            Console.WriteLine($"[SendMessageAsync] Mensagem salva no banco. Id: {message.Id}");

            var result = new MessageDto
            {
                Id = message.Id,
                SenderId = message.Senderid,
                ReceiverId = message.Receiverid,
                Content = message.Content,
                SentAt = message.Sentat,
                SeenAt = message.Seenat,
            };

            Console.WriteLine($"[SendMessageAsync] Retornando DTO. Id: {result.Id}");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SendMessageAsync] Erro ao salvar mensagem: {ex}");
            throw;
        }
    }

    public async Task<IEnumerable<MessageDto>> GetConversationIfParticipantAsync(
        Guid userId,
        Guid otherUserId
    )
    {
        if (userId == otherUserId)
            throw new ConversationAccessDeniedException();

        var messages = await _repo.GetConversationAsync(userId, otherUserId);

        if (!messages.Any())
            return Enumerable.Empty<MessageDto>();

        if (
            messages.Any(m =>
                m.Senderid != userId
                && m.Receiverid != userId
                && m.Senderid != otherUserId
                && m.Receiverid != otherUserId
            )
        )
            return null;

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
