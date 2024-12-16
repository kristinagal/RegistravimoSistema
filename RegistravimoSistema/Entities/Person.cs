using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.Entities
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Vardas { get; set; } = string.Empty;
        [Required] public string Pavarde { get; set; } = string.Empty;

        [Required] public string AsmensKodas { get; set; } = string.Empty;
        [Required] public string TelefonoNumeris { get; set; } = string.Empty;
        [Required] public string ElPastas { get; set; } = string.Empty;

        [Required] public byte[] ProfilioNuotrauka { get; set; } = Array.Empty<byte>();

        // Navigation Property
        [Required] public Guid UserId { get; set; }
        [Required] public User User { get; set; } = null!;

        // One-to-One
        [Required] public Address Address { get; set; } = null!;
    }

}


 