using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatDbContext _db;

    public MessageRepository(ChatDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Message message)
    {
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(Guid user1, Guid user2)
    {
        return await _db
            .Messages.Where(m =>
                (m.Senderid == user1 && m.Receiverid == user2)
                || (m.Senderid == user2 && m.Receiverid == user1)
            )
            .OrderBy(m => m.Sentat)
            .ToListAsync();
    }
}
