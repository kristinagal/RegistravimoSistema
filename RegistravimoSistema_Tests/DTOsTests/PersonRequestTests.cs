using RegistravimoSistema.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RegistravimoSistema.Tests.DTOs
{
    public class PersonRequestTests
    {
        public static IEnumerable<object[]> VardasValidationData =>
            new List<object[]>
            {
                new object[] { null, false },
                new object[] { "", false },
                new object[] { "A", false },
                new object[] { "AB", true },
                new object[] { new string('A', 50), true },
                new object[] { new string('A', 51), false },
            };

        [Theory]
        [MemberData(nameof(VardasValidationData))]
        public void Vardas_ValidationTests(string vardas, bool expectedIsValid)
        {
            var dto = new PersonRequest { Vardas = vardas, Pavarde = "Valid", AsmensKodas = "12345678901", TelefonoNumeris = "867000000", ElPastas = "test@test.com", ProfilioNuotrauka = "Valid", Miestas = "Valid", Gatve = "Valid", NamoNumeris = "1", ButoNumeris = "1" };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        public static IEnumerable<object[]> AsmensKodasValidationData =>
            new List<object[]>
            {
                new object[] { null, false },
                new object[] { "1234567890", false },
                new object[] { "12345678901", true },
                new object[] { "ABCDEFGHIJK", false },
            };

        [Theory]
        [MemberData(nameof(AsmensKodasValidationData))]
        public void AsmensKodas_ValidationTests(string asmensKodas, bool expectedIsValid)
        {
            var dto = new PersonRequest { Vardas = "Valid", Pavarde = "Valid", AsmensKodas = asmensKodas, TelefonoNumeris = "867000000", ElPastas = "test@test.com", ProfilioNuotrauka = "Valid", Miestas = "Valid", Gatve = "Valid", NamoNumeris = "1", ButoNumeris = "1" };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }
    }
}
