using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace Chat.Business.Services.Tests
{
    [TestFixture]
    public class ContactServiceTest
    {
        private Mock<IUserRepository>? _userRepoMock;
        private Mock<IContactRepository>? _contactRepoMock;
        private Mock<IMessageRepository>? _messageRepoMock;
        private ContactService? _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _contactRepoMock = new Mock<IContactRepository>();
            _messageRepoMock = new Mock<IMessageRepository>();
            _service = new ContactService(
                _userRepoMock.Object,
                _contactRepoMock.Object,
                _messageRepoMock.Object
            );
        }

        [Test]
        public async Task AddContactByEmailAsync_ShouldAddContacts_WhenValid()
        {
            var currentUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var email = "test@example.com";
            var targetUser = new User { Id = targetUserId, Email = email };

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(targetUser);
            }

            if (_contactRepoMock != null)
            {
                _contactRepoMock
                    .Setup(r => r.ContactExistsAsync(currentUserId, targetUserId))
                    .ReturnsAsync(false);
            }

            await _service!.AddContactByEmailAsync(currentUserId, email);

            if (_contactRepoMock != null)
            {
                _contactRepoMock.Verify(
                    r =>
                        r.AddAsync(
                            It.Is<Contact>(c =>
                                c.Userid == currentUserId && c.Contactid == targetUserId
                            )
                        ),
                    Times.Once
                );
                _contactRepoMock.Verify(
                    r =>
                        r.AddAsync(
                            It.Is<Contact>(c =>
                                c.Userid == targetUserId && c.Contactid == currentUserId
                            )
                        ),
                    Times.Once
                );
            }
        }

        [Test]
        public void AddContactByEmailAsync_ShouldThrow_WhenUserNotFound()
        {
            var currentUserId = Guid.NewGuid();
            var email = "notfound@example.com";

            if (_userRepoMock != null)
            {
                _userRepoMock
                    .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync((User?)null);
            }

            Assert.ThrowsAsync<Exception>(async () =>
            {
                if (_service != null)
                {
                    await _service.AddContactByEmailAsync(currentUserId, email);
                }
                else
                {
                    throw new Exception("_service is null");
                }
            });
        }

        [Test]
        public void AddContactByEmailAsync_ShouldThrow_WhenAddingSelf()
        {
            var currentUserId = Guid.NewGuid();
            var email = "self@example.com";
            var user = new User { Id = currentUserId, Email = email };

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            }

            Assert.ThrowsAsync<Exception>(async () =>
            {
                if (_service != null)
                {
                    await _service.AddContactByEmailAsync(currentUserId, email);
                }
                else
                {
                    throw new Exception("_service is null");
                }
            });
        }

        [Test]
        public void AddContactByEmailAsync_ShouldThrow_WhenContactAlreadyExists()
        {
            var currentUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var email = "exists@example.com";
            var targetUser = new User { Id = targetUserId, Email = email };

            if (_userRepoMock != null)
            {
                _userRepoMock
                    .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(targetUser);
            }

            if (_contactRepoMock != null)
            {
                _contactRepoMock
                    .Setup(r => r.ContactExistsAsync(currentUserId, targetUserId))
                    .ReturnsAsync(true);
            }

            Assert.ThrowsAsync<Exception>(async () =>
            {
                if (_service != null)
                {
                    await _service.AddContactByEmailAsync(currentUserId, email);
                }
                else
                {
                    throw new Exception("_service is null");
                }
            });
        }

        [Test]
        public async Task GetContactsWithLastMessageAsync_ShouldReturnContactsWithLastMessage()
        {
            var userId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var contactLinks = new List<Contact> { new Contact { Contactid = contactId } };
            var user = new User { Id = contactId, Name = "ContactName" };
            var lastMessage = new Message { Content = "Hello", Sentat = DateTime.UtcNow };

            if (_contactRepoMock != null)
            {
                _contactRepoMock
                    .Setup(r => r.GetContactsRawAsync(userId))
                    .ReturnsAsync(contactLinks);
            }

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.GetByIdAsync(contactId)).ReturnsAsync(user);
            }

            if (_messageRepoMock != null)
            {
                _messageRepoMock
                    .Setup(r => r.GetLastMessageAsync(userId, contactId))
                    .ReturnsAsync(lastMessage);
            }

            var result = await _service!.GetContactsWithLastMessageAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(1));
            var contact = result.First();
            Assert.That(
                contact.GetType().GetProperty("id")?.GetValue(contact),
                Is.EqualTo(contactId)
            );
            Assert.That(
                contact.GetType().GetProperty("name")?.GetValue(contact),
                Is.EqualTo("ContactName")
            );
            Assert.That(
                contact.GetType().GetProperty("lastMessage")?.GetValue(contact),
                Is.EqualTo("Hello")
            );
            Assert.That(
                contact.GetType().GetProperty("lastMessageTime")?.GetValue(contact),
                Is.EqualTo(lastMessage.Sentat)
            );
        }

        [Test]
        public async Task GetContactsWithLastMessageAsync_ShouldSkipNullUsers()
        {
            var userId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var contactLinks = new List<Contact> { new Contact { Contactid = contactId } };

            if (_contactRepoMock != null)
            {
                _contactRepoMock
                    .Setup(r => r.GetContactsRawAsync(userId))
                    .ReturnsAsync(contactLinks);
            }

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.GetByIdAsync(contactId)).ReturnsAsync((User?)null);
            }

            var result = await _service!.GetContactsWithLastMessageAsync(userId);

            Assert.That(result, Is.Empty);
        }
    }
}
