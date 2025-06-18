using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories;

public class MessageRepository(ChatDbContext db) : IMessageRepository
{
    public async Task AddAsync(Message message)
    {
        db.Messages.Add(message);
        await db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(Guid user1, Guid user2)
    {
        return await db
            .Messages.Where(m =>
                (m.Senderid == user1 && m.Receiverid == user2)
                || (m.Senderid == user2 && m.Receiverid == user1)
            )
            .OrderBy(m => m.Sentat)
            .ToListAsync();
    }

    public async Task<Message> GetLastMessageAsync(Guid user1, Guid user2)
    {
        return await db
            .Messages.Where(m =>
                (m.Senderid == user1 && m.Receiverid == user2)
                || (m.Senderid == user2 && m.Receiverid == user1)
            )
            .OrderByDescending(m => m.Sentat)
            .FirstOrDefaultAsync();
    }
}
