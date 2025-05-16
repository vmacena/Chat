using System;
using System.Threading.Tasks;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Interfaces;

namespace Chat.Business.Services;

public class UserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> EmailExistsAsync(string email) => await _repo.EmailExistsAsync(email);

    public async Task<User> RegisterAsync(UserRegisterDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Publicid = Guid.NewGuid().ToString(),
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Createdat = DateTime.UtcNow.ToLocalTime(),
        };
        await _repo.AddAsync(user);
        return user;
    }

    public async Task<User> AuthenticateAsync(UserLoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user == null)
            return null;
        return BCrypt.Net.BCrypt.Verify(dto.Password, user.Passwordhash) ? user : null;
    }
}
