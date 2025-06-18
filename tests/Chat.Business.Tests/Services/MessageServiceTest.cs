using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Chat.Core.Entities;
using Chat.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace Chat.Business.Services.Tests
{
    [TestFixture]
    public class MessageServiceTests
    {
        private Mock<IMessageRepository>? _messageRepoMock;
        private Mock<IUserRepository>? _userRepoMock;
        private MessageService? _service;

        [SetUp]
        public void Setup()
        {
            _messageRepoMock = new Mock<IMessageRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _service = new MessageService(_messageRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public async Task SendMessageAsync_ShouldReturnMessageDto_WhenValid()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello, World!";
            var messageDto = new MessageDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
            };

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.ExistsAsync(senderId)).ReturnsAsync(true);
                _userRepoMock.Setup(r => r.ExistsAsync(receiverId)).ReturnsAsync(true);
            }

            if (_messageRepoMock != null)
            {
                _messageRepoMock
                    .Setup(r => r.AddAsync(It.IsAny<Message>()))
                    .Returns(Task.CompletedTask);
            }

            var result = await _service!.SendMessageAsync(messageDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SenderId, Is.EqualTo(senderId));
            Assert.That(result.ReceiverId, Is.EqualTo(receiverId));
            Assert.That(result.Content, Is.EqualTo(content));
            if (_messageRepoMock != null)
            {
                _messageRepoMock
                    .Setup(r => r.AddAsync(It.IsAny<Message>()))
                    .Returns(Task.CompletedTask);
            }
        }

        [Test]
        public void SendMessageAsync_ShouldThrowException_WhenSenderNotFound()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var messageDto = new MessageDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = "Hello",
            };

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.ExistsAsync(senderId)).ReturnsAsync(false);
                _userRepoMock.Setup(r => r.ExistsAsync(receiverId)).ReturnsAsync(true);
            }

            Assert.ThrowsAsync<Exception>(async () =>
            {
                if (_service != null)
                {
                    await _service.SendMessageAsync(messageDto);
                }
                else
                {
                    throw new Exception("_service is null");
                }
            });
        }

        [Test]
        public void SendMessageAsync_ShouldThrowException_WhenReceiverNotFound()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var messageDto = new MessageDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = "Hello",
            };

            if (_userRepoMock != null)
            {
                _userRepoMock.Setup(r => r.ExistsAsync(senderId)).ReturnsAsync(true);
                _userRepoMock.Setup(r => r.ExistsAsync(receiverId)).ReturnsAsync(false);
            }

            Assert.ThrowsAsync<Exception>(async () =>
            {
                if (_service != null)
                {
                    await _service.SendMessageAsync(messageDto);
                }
                else
                {
                    throw new Exception("_service is null");
                }
            });
        }

        [Test]
        public async Task GetConversationIfParticipantAsync_ShouldReturnMessages_WhenValid()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    Senderid = userId,
                    Receiverid = otherUserId,
                    Content = "Hello",
                    Sentat = DateTime.UtcNow,
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    Senderid = otherUserId,
                    Receiverid = userId,
                    Content = "Hi",
                    Sentat = DateTime.UtcNow,
                },
            };

            if (_messageRepoMock != null)
            {
                _messageRepoMock
                    .Setup(r => r.GetConversationAsync(userId, otherUserId))
                    .ReturnsAsync(messages);
            }

            var result = await _service!.GetConversationIfParticipantAsync(userId, otherUserId);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Content, Is.EqualTo("Hello"));
            Assert.That(result.Last().Content, Is.EqualTo("Hi"));
        }

        [Test]
        public void GetConversationIfParticipantAsync_ShouldThrowException_WhenSameUserIds()
        {
            var userId = Guid.NewGuid();

            Assert.ThrowsAsync<Exception>(
                async () => await _service!.GetConversationIfParticipantAsync(userId, userId)
            );
        }
    }
}
