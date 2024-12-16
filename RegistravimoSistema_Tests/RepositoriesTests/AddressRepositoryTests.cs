using Microsoft.EntityFrameworkCore;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Repositories;
using Xunit;

namespace RegistravimoSistema.Tests.Repositories
{
    public class AddressRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AddressRepository _addressRepository;

        public AddressRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase" + Guid.NewGuid())
                .Options;
            _context = new ApplicationDbContext(options);
            _addressRepository = new AddressRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ValidAddress_AddressIsAdded()
        {
            // Arrange
            var address = new Address
            {
                Miestas = "Kaunas",
                Gatve = "Laisves al.",
                NamoNumeris = "15A",
                ButoNumeris = "2"
            };

            // Act
            await _addressRepository.AddAsync(address);

            // Assert
            var addedAddress = await _context.Addresses.FindAsync(address.Id);
            Assert.NotNull(addedAddress);
            Assert.Equal("Kaunas", addedAddress.Miestas);
        }

        [Fact]
        public async Task AddAsync_NullAddress_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _addressRepository.AddAsync(null);
            });
        }

        [Fact]
        public async Task UpdateAsync_ValidAddress_AddressIsUpdated()
        {
            // Arrange
            var address = new Address { Miestas = "Vilnius", Gatve = "Gedimino pr." };
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            // Act
            address.Miestas = "UpdatedCity";
            await _addressRepository.UpdateAsync(address);

            // Assert
            var updatedAddress = await _context.Addresses.FindAsync(address.Id);
            Assert.Equal("UpdatedCity", updatedAddress.Miestas);
        }

        [Fact]
        public async Task UpdateAsync_AddressNotInDatabase_ThrowsException()
        {
            // Arrange
            var address = new Address { Miestas = "NonExistent", Gatve = "Nowhere St." };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _addressRepository.UpdateAsync(address);
            });
        }

        [Fact]
        public async Task AddAsync_DuplicateAddress_DoesNotThrowButAddsSuccessfully()
        {
            // Arrange
            var address1 = new Address
            {
                Miestas = "Kaunas",
                Gatve = "Duplicate Street",
                NamoNumeris = "5"
            };
            var address2 = new Address
            {
                Miestas = "Kaunas",
                Gatve = "Duplicate Street",
                NamoNumeris = "5"
            };

            // Act
            await _addressRepository.AddAsync(address1);
            await _addressRepository.AddAsync(address2);

            // Assert
            var allAddresses = await _context.Addresses.ToListAsync();
            Assert.Equal(2, allAddresses.Count); // Two addresses exist despite being duplicates.
        }

        [Fact]
        public async Task UpdateAsync_NullAddress_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _addressRepository.UpdateAsync(null);
            });
        }
    }
}