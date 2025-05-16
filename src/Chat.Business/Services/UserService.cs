using System;
using System.Threading.Tasks;
using BCrypt.Net;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Services;

public class UserService
{
    private readonly ChatDbContext _db;

    public UserService(ChatDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> RegisterAsync(UserRegisterDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Publicid = Guid.NewGuid().ToString(),
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // ajuste aqui
            Createdat = DateTime.UtcNow.ToLocalTime(),
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> AuthenticateAsync(UserLoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return null;
        return BCrypt.Net.BCrypt.Verify(dto.Password, user.Passwordhash) ? user : null; // ajuste aqui
    }
}
