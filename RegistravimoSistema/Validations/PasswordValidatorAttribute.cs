using System.ComponentModel.DataAnnotations;

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

            if (password.Length < MinimumLength)
            {
                return new ValidationResult($"Slaptažodis turi būti bent {MinimumLength} simbolių ilgio.");
            }

            return ValidationResult.Success;
        }
    }



}
