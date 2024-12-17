using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RegistravimoSistema.Controllers;
using RegistravimoSistema.DTOs;
using RegistravimoSistema.Entities;
using RegistravimoSistema.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace RegistravimoSistema_Tests.ControllersTests
{
    public class PersonControllerTests
    {
        private readonly Mock<IPersonService> _mockPersonService;
        private readonly PersonController _controller;
        private readonly Guid _currentUserId;

        public PersonControllerTests()
        {
            _mockPersonService = new Mock<IPersonService>();

            // Mock user authentication
            _currentUserId = Guid.NewGuid();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _currentUserId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.User).Returns(principal);

            var mockControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            _controller = new PersonController(_mockPersonService.Object)
            {
                ControllerContext = mockControllerContext
            };
        }

        // ------------- CreatePerson Tests -------------
        [Fact]
        public async Task CreatePerson_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new PersonRequest { Vardas = "John", Pavarde = "Doe" };
            _mockPersonService
                .Setup(s => s.GetPersonByUserIdAsync(_currentUserId))
                .ReturnsAsync((Person)null); // No existing person
            _mockPersonService
                .Setup(s => s.CreatePersonAsync(It.IsAny<PersonRequest>(), _currentUserId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreatePerson(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetPersonById), createdResult.ActionName);
            Assert.Equal(request, createdResult.Value);
        }

        [Fact]
        public async Task CreatePerson_ExistingPerson_ReturnsConflict()
        {
            // Arrange
            var request = new PersonRequest { Vardas = "John", Pavarde = "Doe" };
            var existingPerson = new Person { Id = Guid.NewGuid(), UserId = _currentUserId };
            _mockPersonService
                .Setup(s => s.GetPersonByUserIdAsync(_currentUserId))
                .ReturnsAsync(existingPerson);

            // Act
            var result = await _controller.CreatePerson(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("You have already created a person. A user can only create one person.", conflictResult.Value);
        }

        // ------------- GetPersonById Tests -------------
        [Fact]
        public async Task GetPersonById_ValidId_ReturnsPerson()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var person = new Person
            {
                Id = personId,
                Vardas = "John",
                Pavarde = "Doe",
                UserId = _currentUserId,
                Address = new Address { Miestas = "Vilnius", Gatve = "Main", NamoNumeris = "1", ButoNumeris = "2" }
            };
            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId)).ReturnsAsync(person);

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PersonResponse>(okResult.Value);
            Assert.Equal("John", response.Vardas);
            Assert.Equal("Doe", response.Pavarde);
            Assert.Equal("Vilnius", response.Address.Miestas);
        }

        [Fact]
        public async Task GetPersonById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId)).ReturnsAsync((Person)null);

            // Act
            var result = await _controller.GetPersonById(personId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Person not found.", notFoundResult.Value);
        }

        // ------------- UpdateField Tests -------------
        [Fact]
        public async Task UpdateField_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var requestValue = "UpdatedValue";
            var fieldName = "Vardas";

            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId))
                              .ReturnsAsync(new Person { UserId = _currentUserId });
            _mockPersonService.Setup(s => s.UpdateFieldAsync(personId, fieldName, requestValue, _currentUserId))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateVardas(personId, new UpdateVardasRequest { Vardas = requestValue });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateField_PersonNotFound_ReturnsNotFound()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId))
                              .ReturnsAsync((Person)null);

            // Act
            var result = await _controller.UpdateVardas(personId, new UpdateVardasRequest { Vardas = "UpdatedValue" });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Person not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateField_UserNotAuthorized_ReturnsForbid()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId))
                              .ReturnsAsync(new Person { UserId = Guid.NewGuid() }); // Different UserId

            // Act
            var result = await _controller.UpdateVardas(personId, new UpdateVardasRequest { Vardas = "UpdatedValue" });

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateField_InvalidFieldValue_ReturnsBadRequest()
        {
            // Arrange
            var personId = Guid.NewGuid();
            _mockPersonService.Setup(s => s.GetPersonByIdAsync(personId))
                              .ReturnsAsync(new Person { UserId = _currentUserId });
            _mockPersonService.Setup(s => s.UpdateFieldAsync(personId, "Vardas", "", _currentUserId))
                              .ThrowsAsync(new ArgumentException("Invalid field value"));

            // Act
            var result = await _controller.UpdateVardas(personId, new UpdateVardasRequest { Vardas = "" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid field value", badRequestResult.Value);
        }
    }
}
