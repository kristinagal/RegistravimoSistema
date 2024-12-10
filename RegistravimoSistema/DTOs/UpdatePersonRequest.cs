namespace RegistravimoSistema.DTOs
{
    public class UpdatePersonRequest
    {
        public string Vardas { get; set; }
        public string Pavarde { get; set; }
        public string AsmensKodas { get; set; }
        public string TelefonoNumeris { get; set; }
        public string ElPastas { get; set; }
        public string Miestas { get; set; }
        public string Gatve { get; set; }
        public string NamoNumeris { get; set; }
        public string ButoNumeris { get; set; }
        public string? ProfilioNuotrauka { get; set; } // Base64 string
    }

}
