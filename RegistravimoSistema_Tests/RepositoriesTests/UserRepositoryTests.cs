using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Repositories;
using Xunit;

namespace RegistravimoSistema.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid())
                .Options;
            _context = new ApplicationDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ValidUser_UserIsAdded()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 },
                Role = "Admin"
            };

            // Act
            await _userRepository.AddAsync(user);

            // Assert
            var addedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(addedUser);
            Assert.Equal("testuser", addedUser.Username);
        }

        [Fact]
        public async Task AddAsync_NullUser_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _userRepository.AddAsync(null);
            });
        }

        [Fact]
        public async Task GetByUsernameAsync_ValidUsername_ReturnsUser()
        {
            // Arrange
            var user = new User { Username = "uniqueUser" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByUsernameAsync("uniqueUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("uniqueUser", result.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_InvalidUsername_ReturnsNull()
        {
            // Act
            var result = await _userRepository.GetByUsernameAsync("nonexistentUser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_NullUsername_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _userRepository.GetByUsernameAsync(null);
            });
        }

        [Fact]
        public async Task DeleteAsync_ValidId_RemovesUser()
        {
            // Arrange
            var user = new User { Username = "toDelete" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(user.Id);

            // Assert
            Assert.Null(await _context.Users.FindAsync(user.Id));
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _userRepository.DeleteAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsUser()
        {
            // Arrange
            var user = new User { Username = "existingUser" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _userRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
