using Moq;
using RegistravimoSistema.Services;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.Entities;


namespace RegistravimoSistema_Tests.ServicesTests
{
    public class AccountServiceTests
    {
        private readonly AccountService _accountService;
        private readonly Mock<IPersonRepository> _mockPersonRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;

        public AccountServiceTests()
        {
            _mockPersonRepo = new Mock<IPersonRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _accountService = new AccountService(_mockPersonRepo.Object, _mockUserRepo.Object);
        }

        [Fact]
        public void CreatePasswordHash_ValidPassword_ReturnsHashAndSalt()
        {
            // Arrange
            var password = "StrongPassword123!";

            // Act
            _accountService.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            // Assert
            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.NotEmpty(hash);
            Assert.NotEmpty(salt);
        }

        [Fact]
        public void VerifyPasswordHash_ValidPassword_ReturnsTrue()
        {
            // Arrange
            var password = "StrongPassword123!";
            _accountService.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            // Act
            var result = _accountService.VerifyPasswordHash(password, hash, salt);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordHash_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var password = "StrongPassword123!";
            _accountService.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            // Act
            var result = _accountService.VerifyPasswordHash("WrongPassword", hash, salt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserWithPersonAsync_ValidUserId_DeletesUserAndPerson()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockPersonRepo.Setup(repo => repo.GetByUserIdAsync(userId))
                           .ReturnsAsync(new Person { Id = Guid.NewGuid(), UserId = userId });
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId))
                         .ReturnsAsync(new User { Id = userId });

            // Act
            await _accountService.DeleteUserWithPersonAsync(userId);

            // Assert
            _mockPersonRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
            _mockUserRepo.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserWithPersonAsync_InvalidUserId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId))
                         .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _accountService.DeleteUserWithPersonAsync(userId));
        }
    }
}
