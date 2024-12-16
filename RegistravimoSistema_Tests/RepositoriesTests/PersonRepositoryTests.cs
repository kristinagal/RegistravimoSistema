using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Repositories;
using Xunit;

namespace RegistravimoSistema.Tests.Repositories
{
    public class PersonRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PersonRepository _personRepository;

        public PersonRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid())
                .Options;
            _context = new ApplicationDbContext(options);
            _personRepository = new PersonRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ValidPerson_PersonIsAdded()
        {
            // Arrange
            var person = new Person
            {
                Vardas = "Jonas",
                Pavarde = "Jonaitis",
                AsmensKodas = "12345678901",
                TelefonoNumeris = "867000000",
                ElPastas = "jonas@example.com"
            };

            // Act
            await _personRepository.AddAsync(person);

            // Assert
            var addedPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id);
            Assert.NotNull(addedPerson);
            Assert.Equal("Jonas", addedPerson.Vardas);
        }

        [Fact]
        public async Task AddAsync_NullPerson_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personRepository.AddAsync(null);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsPerson()
        {
            // Arrange
            var person = new Person { Vardas = "Petras", Pavarde = "Petraitis" };
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _personRepository.GetByIdAsync(person.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Petras", result.Vardas);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _personRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ValidUserId_ReturnsPerson()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var person = new Person { Vardas = "Lina", UserId = userId };
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _personRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetByUserIdAsync_InvalidUserId_ReturnsNull()
        {
            // Act
            var result = await _personRepository.GetByUserIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_RemovesPerson()
        {
            // Arrange
            var person = new Person { Vardas = "DeleteMe" };
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            await _personRepository.DeleteAsync(person.Id);

            // Assert
            Assert.Null(await _context.Persons.FindAsync(person.Id));
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personRepository.DeleteAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task UpdateAsync_ValidPerson_UpdatesPerson()
        {
            // Arrange
            var person = new Person
            {
                Vardas = "InitialName",
                Pavarde = "InitialSurname",
                AsmensKodas = "12345678901",
                TelefonoNumeris = "867000000",
                ElPastas = "initial@example.com"
            };
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            person.Vardas = "UpdatedName";
            await _personRepository.UpdateAsync(person);

            // Assert
            var updatedPerson = await _context.Persons.FindAsync(person.Id);
            Assert.Equal("UpdatedName", updatedPerson.Vardas);
        }

        [Fact]
        public async Task UpdateAsync_NullPerson_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personRepository.UpdateAsync(null);
            });
        }
    }
}
