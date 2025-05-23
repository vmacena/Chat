using Chat.Core.Entities;

namespace Chat.Core.Interfaces;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task<IEnumerable<Message>> GetConversationAsync(Guid user1, Guid user2);
    Task<Message?> GetLastMessageAsync(Guid user1, Guid user2);
}
