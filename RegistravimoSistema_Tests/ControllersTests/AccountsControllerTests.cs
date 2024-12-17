using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RegistravimoSistema.Controllers;
using RegistravimoSistema.Services;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Mappers;

namespace RegistravimoSistema_Tests.ControllersTests
{
    public class AccountsControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IAccountMapper> _mockAccountMapper;

        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAccountService = new Mock<IAccountService>();
            _mockJwtService = new Mock<IJwtService>();
            _mockAccountMapper = new Mock<IAccountMapper>();

            _controller = new AccountsController(
                _mockUserRepository.Object,
                _mockJwtService.Object,
                _mockAccountService.Object,
                _mockAccountMapper.Object
            );
        }

        // ----------- SignUp Tests -----------

        [Fact]
        public async Task SignUp_WhenValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = new UserAuthRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync((User)null);

            _mockAccountMapper.Setup(m => m.MapFromDto(request))
                .Returns(new User { Username = request.Username });

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SignUp(request);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task SignUp_WhenUsernameExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserAuthRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync(new User { Username = request.Username });

            // Act
            var result = await _controller.SignUp(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists.", badRequestResult.Value);
        }

        // ----------- Login Tests -----------

        [Fact]
        public async Task Login_WhenValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new UserAuthRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            var user = new User
            {
                Username = request.Username,
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 }
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync(user);

            _mockAccountService.Setup(s => s.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                .Returns(true);

            _mockJwtService.Setup(s => s.GenerateJwtToken(user))
                .Returns("fake-jwt-token");

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Manually extract Token using dictionary or anonymous handling
            var value = okResult.Value;
            var tokenProperty = value.GetType().GetProperty("Token");
            var tokenValue = tokenProperty.GetValue(value, null);

            Assert.NotNull(tokenValue);
            Assert.Equal("fake-jwt-token", tokenValue);
        }




        [Fact]
        public async Task Login_WhenUserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var request = new UserAuthRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not found.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WhenPasswordIncorrect_ReturnsUnauthorized()
        {
            // Arrange
            var request = new UserAuthRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Username = request.Username,
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 }
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync(user);

            _mockAccountService.Setup(s => s.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                .Returns(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Incorrect password.", unauthorizedResult.Value);
        }

        // ----------- DeleteUser Tests -----------

        [Fact]
        public async Task DeleteUser_WhenUserExists_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _mockAccountService.Setup(s => s.DeleteUserWithPersonAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteUser_WhenExceptionThrown_ReturnsServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _mockAccountService.Setup(s => s.DeleteUserWithPersonAsync(userId))
                .Throws(new Exception("Some error occurred."));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Contains("Some error occurred.", serverErrorResult.Value.ToString());
        }
    }
}
