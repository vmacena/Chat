using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Chat.Business.Services
{
    public class UploadfileService(
        IUploadfileRepository repo,
        IWebHostEnvironment env,
        IMessageRepository messageRepo
    )
    {
        public async Task<UploadfileDto> SaveAsync(IFormFile file, Guid senderId, Guid receiverId)
        {
            var messageId = Guid.NewGuid();
            var message = new Message
            {
                Id = messageId,
                Senderid = senderId,
                Receiverid = receiverId,
                Content = $"{file.FileName}",
                Sentat = DateTime.UtcNow,
            };

            await messageRepo.AddAsync(message);

            var webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            if (!Directory.Exists(webRootPath))
                Directory.CreateDirectory(webRootPath);

            var uploadsDir = Path.Combine(webRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(uploadsDir, fileName);

            await using (var stream = File.Create(path))
                await file.CopyToAsync(stream);

            var url = $"/uploads/{fileName}";

            var upload = new Uploadfile
            {
                Id = Guid.NewGuid(),
                Messageid = messageId,
                Mimetype = file.ContentType,
                Url = url,
                Size = file.Length,
                Uploadedat = DateTime.UtcNow.ToLocalTime(),
            };

            await repo.AddAsync(upload);

            return new UploadfileDto
            {
                Id = upload.Id,
                MessageId = upload.Messageid,
                Mimetype = upload.Mimetype!,
                Url = upload.Url!,
                Size = upload.Size,
                UploadedAt = upload.Uploadedat,
            };
        }

        public async Task<IEnumerable<UploadfileDto>> GetByMessageAsync(Guid messageId)
        {
            var list = await repo.GetByMessageIdAsync(messageId);
            return list.Select(u => new UploadfileDto
            {
                Id = u.Id,
                MessageId = u.Messageid,
                Mimetype = u.Mimetype!,
                Url = u.Url!,
                Size = u.Size,
                UploadedAt = u.Uploadedat,
            });
        }
    }
}
