using RegistravimoSistema.Validations;
using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.DTOs
{
    public class UserAuthRequest
    {
        /// <summary>
        /// Naudotojo vardas
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [TextLengthValidator(3, 50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Slaptažodis
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [PasswordValidator]
        public string Password { get; set; } = string.Empty;
    }
}
