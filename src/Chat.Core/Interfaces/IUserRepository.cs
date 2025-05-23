using Chat.Core.Entities;

namespace Chat.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> ExistsAsync(Guid id);
    Task<List<User>> GetContactsAsync(Guid userId);
    Task<bool> ContactExistsAsync(Guid userId, Guid contactId);
    Task AddContactRawAsync(Contact contact);
    Task AddAsync(User user);
}
