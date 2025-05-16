using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ChatDbContext _db;

    public UserRepository(ChatDbContext db)
    {
        _db = db;
    }

    public async Task<User> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _db.Users.AnyAsync(u => u.Email == email);
}
