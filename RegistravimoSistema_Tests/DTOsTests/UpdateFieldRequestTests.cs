using System.ComponentModel.DataAnnotations;
using RegistravimoSistema.DTOs;
using Xunit;

namespace RegistravimoSistema.Tests.DTOs
{
    public class UpdateFieldRequestTests
    {
        // Predefined constant strings for validation boundaries
        private static class TestData
        {
            public const string BelowMinimum = "a"; // Below minimum length (1 character)
            public const string MinimumLength = "aa"; // Minimum valid length (2 characters)
            public const string MiddleRange = "a" + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 48 characters
            public const string MaximumLength = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 50 characters
            public const string AboveMaximum = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 51 characters
        }

        // UpdateVardasRequest Tests
        [Theory]
        [InlineData(null, false)] // Null input
        [InlineData(TestData.BelowMinimum, false)] // Below minimum length
        [InlineData(TestData.MinimumLength, true)] // Minimum valid length
        [InlineData(TestData.MiddleRange, true)]   // Middle range
        [InlineData(TestData.MaximumLength, true)] // Maximum valid length
        [InlineData(TestData.AboveMaximum, false)] // Above maximum length
        public void UpdateVardasRequest_ValidationTests(string vardas, bool expectedIsValid)
        {
            // Arrange
            var dto = new UpdateVardasRequest { Vardas = vardas };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.Equal(expectedIsValid, result);
        }

        // UpdatePavardeRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData(TestData.BelowMinimum, false)]
        [InlineData(TestData.MinimumLength, true)]
        [InlineData(TestData.MiddleRange, true)]
        [InlineData(TestData.MaximumLength, true)]
        [InlineData(TestData.AboveMaximum, false)]
        public void UpdatePavardeRequest_ValidationTests(string pavarde, bool expectedIsValid)
        {
            var dto = new UpdatePavardeRequest { Pavarde = pavarde };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        // UpdateAsmensKodasRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData("1234567890", false)] // Less than 11 characters
        [InlineData("12345678901", true)] // Exactly 11 characters
        [InlineData("123456789012", false)] // More than 11 characters
        public void UpdateAsmensKodasRequest_ValidationTests(string asmensKodas, bool expectedIsValid)
        {
            var dto = new UpdateAsmensKodasRequest { AsmensKodas = asmensKodas };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        // UpdateTelefonoNumerisRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData("12345", false)] // Invalid length
        [InlineData("+37060012345", true)] // Valid format
        [InlineData("860012345", true)] // Valid Lithuanian number
        [InlineData("abcd123456", false)] // Invalid format
        public void UpdateTelefonoNumerisRequest_ValidationTests(string telefonoNumeris, bool expectedIsValid)
        {
            var dto = new UpdateTelefonoNumerisRequest { TelefonoNumeris = telefonoNumeris };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        // UpdateElPastasRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData("invalidEmail", false)]
        [InlineData("test@domain.com", true)]
        [InlineData("user@domain", false)]
        public void UpdateElPastasRequest_ValidationTests(string elPastas, bool expectedIsValid)
        {
            var dto = new UpdateElPastasRequest { ElPastas = elPastas };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        // UpdateProfilioNuotraukaRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)] // Empty string
        [InlineData("ValidBase64String==", true)] // Simulating valid base64 string
        public void UpdateProfilioNuotraukaRequest_ValidationTests(string profilioNuotrauka, bool expectedIsValid)
        {
            var dto = new UpdateProfilioNuotraukaRequest { ProfilioNuotrauka = profilioNuotrauka };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        // UpdateMiestasRequest Tests
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Vilnius", true)]
        public void UpdateMiestasRequest_ValidationTests(string miestas, bool expectedIsValid)
        {
            var dto = new UpdateMiestasRequest { Miestas = miestas };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

    }
}
