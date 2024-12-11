using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RegistravimoSistema.Validations
{
    // Validator for ElPastas (Email)
    public class EmailValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new ValidationResult("El. paštas yra netinkamo formato.");
            }
            return ValidationResult.Success;
        }
    }



}
