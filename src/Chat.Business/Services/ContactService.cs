using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;

namespace Chat.Business.Services;

public class ContactService
{
    private readonly IUserRepository _userRepo;
    private readonly IContactRepository _contactRepo;
    private readonly IMessageRepository _messageRepo;

    public ContactService(
        IUserRepository userRepo,
        IContactRepository contactRepo,
        IMessageRepository messageRepo
    )
    {
        _userRepo = userRepo;
        _contactRepo = contactRepo;
        _messageRepo = messageRepo;
    }

    public async Task AddContactByEmailAsync(Guid currentUserId, string email)
    {
        var target =
            await _userRepo.GetByEmailAsync(email.Trim().ToLower())
            ?? throw new Exception("Contato não encontrado.");

        if (target.Id == currentUserId)
            throw new Exception("Você não pode se adicionar.");

        if (await _contactRepo.ContactExistsAsync(currentUserId, target.Id))
            throw new Exception("Contato já adicionado.");

        var c1 = new Contact
        {
            Id = Guid.NewGuid(),
            Userid = currentUserId,
            Contactid = target.Id,
            Status = "accepted",
            Createdat = DateTime.UtcNow.ToLocalTime(),
        };

        var c2 = new Contact
        {
            Id = Guid.NewGuid(),
            Userid = target.Id,
            Contactid = currentUserId,
            Status = "accepted",
            Createdat = DateTime.UtcNow.ToLocalTime(),
        };

        await _contactRepo.AddAsync(c1);
        await _contactRepo.AddAsync(c2);
    }

    public async Task<List<object>> GetContactsWithLastMessageAsync(Guid userId)
    {
        var contactLinks = await _contactRepo.GetContactsRawAsync(userId);
        var contactIds = contactLinks.Select(c => c.Contactid).ToList();

        var result = new List<object>();
        foreach (var contactId in contactIds)
        {
            var user = await _userRepo.GetByIdAsync(contactId);
            if (user == null)
                continue;

            var last = await _messageRepo.GetLastMessageAsync(userId, contactId);
            result.Add(
                new
                {
                    id = user.Id,
                    name = user.Name,
                    lastMessage = last?.Content,
                    lastMessageTime = last?.Sentat,
                }
            );
        }

        return result;
    }
}
