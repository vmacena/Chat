using Chat.Core.Persistence;

namespace Chat.Business
{
    public class MessageBusiness(ChatDbContext dbContext)
    {
        private readonly ChatDbContext _dbContext = dbContext;

        public IQueryable<object> GetAllMessages()
        {
            return _dbContext
                .Messages.OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    user = m.User ?? "",
                    content = m.Content ?? "",
                    timestamp = m.Timestamp,
                });
        }
    }
}
