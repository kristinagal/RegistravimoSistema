using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        [Required] public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        [Required] public string Role { get; set; } = "User"; 

        // Navigation Property
        public ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}

