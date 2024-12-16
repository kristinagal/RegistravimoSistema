using RegistravimoSistema.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RegistravimoSistema.Tests.DTOs
{
    public class UserAuthRequestTests
    {
        public static IEnumerable<object[]> UsernameValidationData =>
            new List<object[]>
            {
                new object[] { null, false },
                new object[] { "", false },
                new object[] { "AB", false },
                new object[] { "ABC", true },
                new object[] { new string('A', 50), true },
                new object[] { new string('A', 51), false },
            };

        [Theory]
        [MemberData(nameof(UsernameValidationData))]
        public void Username_ValidationTests(string username, bool expectedIsValid)
        {
            var dto = new UserAuthRequest { Username = username, Password = "ValidPassword123!" };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }

        public static IEnumerable<object[]> PasswordValidationData =>
            new List<object[]>
            {
                new object[] { null, false },
                new object[] { "123", false },
                new object[] { "Valid123!", true },
                new object[] { "short!", false },
            };

        [Theory]
        [MemberData(nameof(PasswordValidationData))]
        public void Password_ValidationTests(string password, bool expectedIsValid)
        {
            var dto = new UserAuthRequest { Username = "ValidUser", Password = password };
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, result);
        }
    }
}
