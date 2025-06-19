using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Chat.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chat.Business.Repositories
{
    public class UploadfileRepository(ChatDbContext db) : IUploadfileRepository
    {
        public async Task AddAsync(Uploadfile uploadfile)
        {
            db.Uploadfiles.Add(uploadfile);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Uploadfile>> GetByMessageIdAsync(Guid messageId)
        {
            return await db.Uploadfiles.Where(u => u.Messageid == messageId).ToListAsync();
        }
    }
}
