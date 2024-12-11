using RegistravimoSistema.Validations;

namespace RegistravimoSistema.DTOs
{
    public class UserAuthRequest
    {
        [TextLengthValidator(3, 50)]
        public string Username { get; set; } = string.Empty;

        [PasswordValidator]
        public string Password { get; set; } = string.Empty;
    }
}
