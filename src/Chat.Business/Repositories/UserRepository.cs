using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories;

public class UserRepository(ChatDbContext db) : IUserRepository
{
    public async Task<User> GetByEmailAsync(string email) =>
        await db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await db.Users.AnyAsync(u => u.Email == email);

    public async Task<User> GetByIdAsync(Guid id) => await db.Users.FindAsync(id);

    public async Task<bool> ExistsAsync(Guid id) => await db.Users.AnyAsync(u => u.Id == id);

    public async Task<List<User>> GetContactsAsync(Guid userId)
    {
        return await db
            .Contacts.Where(c => c.Userid == userId)
            .Include(c => c.ContactNavigation)
            .Select(c => c.ContactNavigation)
            .ToListAsync();
    }

    public async Task<bool> ContactExistsAsync(Guid userId, Guid contactId)
    {
        return await db.Contacts.AnyAsync(c => c.Userid == userId && c.Contactid == contactId);
    }

    public async Task AddContactRawAsync(Contact contact)
    {
        db.Contacts.Add(contact);
        await db.SaveChangesAsync();
    }
}
