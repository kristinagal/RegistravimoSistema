using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.Validations
{
    // Validator for text length (Vardas, Pavarde)
    public class TextLengthValidatorAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public TextLengthValidatorAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string text && (text.Length < _minLength || text.Length > _maxLength))
            {
                return new ValidationResult($"Laukas turi būti tarp {_minLength} ir {_maxLength} simbolių.");
            }
            return ValidationResult.Success;
        }
    }

}
