using Chat.Core.Entities;

namespace Chat.Core.Interfaces
{
    public interface IUploadfileRepository
    {
        Task AddAsync(Uploadfile uploadfile);
        Task<IEnumerable<Uploadfile>> GetByMessageIdAsync(Guid messageId);
    }
}
