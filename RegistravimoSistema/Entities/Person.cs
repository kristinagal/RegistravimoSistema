namespace RegistravimoSistema.Entities
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Vardas { get; set; } = string.Empty;
        public string Pavarde { get; set; } = string.Empty;

        public string AsmensKodas { get; set; } = string.Empty;
        public string TelefonoNumeris { get; set; } = string.Empty;
        public string ElPastas { get; set; } = string.Empty;

        public byte[] ProfilioNuotrauka { get; set; } = Array.Empty<byte>();

        // Navigation Property
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // One-to-One
        public Address Address { get; set; } = null!;
    }

}


 