using Chat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat.Core.Persistence
{
    public class ChatDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
