using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RegistravimoSistema.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PasswordValidatorAttribute : ValidationAttribute
    {
        public int MinimumLength { get; set; } = 8;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Slaptažodis yra privalomas.");
            }

            var password = value.ToString();

            // Check minimum length
            if (password.Length < MinimumLength)
            {
                return new ValidationResult($"Slaptažodis turi būti bent {MinimumLength} simbolių ilgio.");
            }

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Slaptažodyje turi būti bent viena didžioji raidė.");
            }

            // Check for at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Slaptažodyje turi būti bent viena mažoji raidė.");
            }

            // Check for at least one number
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new ValidationResult("Slaptažodyje turi būti bent vienas skaičius.");
            }

            // Check for at least one special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]"))
            {
                return new ValidationResult("Slaptažodyje turi būti bent vienas specialusis simbolis (!@#$%^&* ir kt.).");
            }

            return ValidationResult.Success;
        }
    }
}
