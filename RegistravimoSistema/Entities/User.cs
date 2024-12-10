using System;

namespace RegistravimoSistema.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Role { get; set; } = "User"; // Default role

        // Navigation Property
        public ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}

