using Moq;
using RegistravimoSistema.Services;
using RegistravimoSistema.Repositories;
using RegistravimoSistema.Mappers;
using RegistravimoSistema.Entities;

namespace RegistravimoSistema_Tests.ServicesTests
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _mockPersonRepo;
        private readonly Mock<IAddressRepository> _mockAddressRepo;
        private readonly Mock<IPersonMapper> _mockMapper;
        private readonly PersonService _personService;

        public PersonServiceTests()
        {
            _mockPersonRepo = new Mock<IPersonRepository>();
            _mockAddressRepo = new Mock<IAddressRepository>();
            _mockMapper = new Mock<IPersonMapper>();
            _personService = new PersonService(_mockPersonRepo.Object, _mockAddressRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreatePersonAsync_ValidRequest_CreatesPersonAndAddress()
        {
            // Arrange
            var mockPersonRepository = new Mock<IPersonRepository>();
            var mockAddressRepository = new Mock<IAddressRepository>();
            var mockPersonMapper = new Mock<IPersonMapper>();

            // Setup repository and mapper behavior
            mockPersonRepository.Setup(r => r.AddAsync(It.IsAny<Person>())).Returns(Task.CompletedTask);
            mockAddressRepository.Setup(r => r.AddAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);
            mockPersonMapper.Setup(m => m.MapFromDto(It.IsAny<PersonRequest>(), It.IsAny<Guid>()))
                .Returns(new Person
                {
                    Vardas = "John",
                    Pavarde = "Doe",
                    AsmensKodas = "12345678901"
                });
            mockPersonMapper.Setup(m => m.MapAddressFromDto(It.IsAny<PersonRequest>(), It.IsAny<Guid>()))
                .Returns(new Address());

            // valid image from file converted to Base64
            string imagePath = @"C:\Users\Studentas\source\repos\RegistravimoSistema\RegistravimoSistema\Screenshot 2024-11-27 210003.png";
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string validBase64Png = Convert.ToBase64String(imageBytes);

            var request = new PersonRequest
            {
                ProfilioNuotrauka = validBase64Png,
                Vardas = "John",
                Pavarde = "Doe",
                AsmensKodas = "12345678901",
                TelefonoNumeris = "861234567",
                ElPastas = "john.doe@example.com",
                Miestas = "Vilnius",
                Gatve = "Main",
                NamoNumeris = "1",
                ButoNumeris = "1"
            };

            var personService = new PersonService(
                mockPersonRepository.Object,
                mockAddressRepository.Object,
                mockPersonMapper.Object
            );

            // No assignment since method is void
            await personService.CreatePersonAsync(request, Guid.NewGuid());

            // Verify that repositories were called correctly
            mockPersonRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Once);
            mockAddressRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
        }


        [Fact]
        public async Task UpdateFieldAsync_InvalidField_ThrowsArgumentException()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var person = new Person { UserId = userId };

            _mockPersonRepo.Setup(repo => repo.GetByIdAsync(personId)).ReturnsAsync(person);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _personService.UpdateFieldAsync(personId, "InvalidField", "Value", userId));
        }
    }
}
