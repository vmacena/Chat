using Chat.Core.Entities;

namespace Chat.Core.Interfaces;

public interface IContactRepository
{
    Task<List<User>> GetContactsAsync(Guid userId);
    Task<bool> ContactExistsAsync(Guid userId, Guid contactId);
    Task AddAsync(Contact contact);
    Task<List<Contact>> GetContactsRawAsync(Guid userId);
}
