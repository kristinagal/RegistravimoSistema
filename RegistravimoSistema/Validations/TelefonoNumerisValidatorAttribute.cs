using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RegistravimoSistema.Validations
{
    // Validator for TelefonoNumeris
    public class TelefonoNumerisValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string telefonoNumeris &&
                !Regex.IsMatch(telefonoNumeris, @"^(86\d{7}|\+3706\d{7})$"))
            {
                return new ValidationResult("Telefono numeris turi būti formato '86*******' arba '+3706*******'.");
            }
            return ValidationResult.Success;
        }
    }



}
