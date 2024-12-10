namespace RegistravimoSistema.DTOs
{
    public class CreatePersonRequest
    {
        public string Vardas { get; set; } = string.Empty;
        public string Pavarde { get; set; } = string.Empty;
        public string AsmensKodas { get; set; } = string.Empty;
        public string TelefonoNumeris { get; set; } = string.Empty;
        public string ElPastas { get; set; } = string.Empty;
        public string? ProfilioNuotrauka { get; set; } // Base64 string
        public string Miestas { get; set; } = string.Empty;
        public string Gatve { get; set; } = string.Empty;
        public string NamoNumeris { get; set; } = string.Empty;
        public string? ButoNumeris { get; set; }
    }

}
