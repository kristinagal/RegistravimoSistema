namespace RegistravimoSistema.DTOs
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string Vardas { get; set; } = string.Empty;
        public string Pavarde { get; set; } = string.Empty;
        public string AsmensKodas { get; set; } = string.Empty;
        public string TelefonoNumeris { get; set; } = string.Empty;
        public string ElPastas { get; set; } = string.Empty;
        public string? ProfilioNuotrauka { get; set; } // Base64 encoded string
        public AddressResponse Address { get; set; } = null!;
    }
}
