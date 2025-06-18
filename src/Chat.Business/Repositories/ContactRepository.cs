using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories;

public class ContactRepository(ChatDbContext db) : IContactRepository
{
    public async Task<List<User>> GetContactsAsync(Guid userId)
    {
        return await db
            .Contacts.Where(c => c.Userid == userId)
            .Include(c => c.ContactNavigation)
            .Select(c => c.ContactNavigation)
            .ToListAsync();
    }

    public async Task<List<Contact>> GetContactsRawAsync(Guid userId)
    {
        return await db.Contacts.Where(c => c.Userid == userId).ToListAsync();
    }

    public async Task<bool> ContactExistsAsync(Guid userId, Guid contactId)
    {
        return await db.Contacts.AnyAsync(c => c.Userid == userId && c.Contactid == contactId);
    }

    public async Task AddAsync(Contact contact)
    {
        db.Contacts.Add(contact);
        await db.SaveChangesAsync();
    }
}
