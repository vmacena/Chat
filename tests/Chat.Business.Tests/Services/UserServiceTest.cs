using System;
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
    public class UserServiceTests
    {
        private Mock<IUserRepository>? _userRepoMock;
        private UserService? _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _service = new UserService(_userRepoMock.Object);
        }

        [Test]
        public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            var email = "test@example.com";
            _userRepoMock!.Setup(r => r.EmailExistsAsync(email)).ReturnsAsync(true);
            var result = await _service!.EmailExistsAsync(email);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            var email = "notfound@example.com";
            _userRepoMock!.Setup(r => r.EmailExistsAsync(email)).ReturnsAsync(false);
            var result = await _service!.EmailExistsAsync(email);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RegisterAsync_ShouldReturnUser_WhenValid()
        {
            var dto = new UserRegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
            };
            _userRepoMock!.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            var result = await _service!.RegisterAsync(dto);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Email, Is.EqualTo(dto.Email));
            Assert.That(result.Passwordhash, Is.Not.Null.And.Not.Empty);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Passwordhash = hashedPassword,
            };
            _userRepoMock!.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var dto = new UserLoginDto { Email = email, Password = password };
            var result = await _service!.AuthenticateAsync(dto);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var email = "notfound@example.com";
            var password = "password123";
            _userRepoMock!.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
            var dto = new UserLoginDto { Email = email, Password = password };
            var result = await _service!.AuthenticateAsync(dto);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            var email = "test@example.com";
            var password = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Passwordhash = hashedPassword,
            };
            _userRepoMock!.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            var dto = new UserLoginDto { Email = email, Password = password };
            var result = await _service!.AuthenticateAsync(dto);
            Assert.That(result, Is.Null);
        }
    }
}
