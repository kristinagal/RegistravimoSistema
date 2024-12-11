using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RegistravimoSistema.Validations
{
    // Validator for AsmensKodas (11 digits)
    public class AsmensKodasValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string asmensKodas && !Regex.IsMatch(asmensKodas, @"^\d{11}$"))
            {
                return new ValidationResult("Asmens kodas turi būti lygiai 11 skaitmenų.");
            }
            return ValidationResult.Success;
        }
    }



}
