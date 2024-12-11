using RegistravimoSistema.Validations;

namespace RegistravimoSistema.DTOs
{
    public class CreatePersonRequest
    {
        [TextLengthValidator(2, 50)]
        public string Vardas { get; set; } = string.Empty;

        [TextLengthValidator(2, 50)]
        public string Pavarde { get; set; } = string.Empty;

        [AsmensKodasValidator]
        public string AsmensKodas { get; set; } = string.Empty;

        [TelefonoNumerisValidator]
        public string TelefonoNumeris { get; set; } = string.Empty;

        [EmailValidator]
        public string ElPastas { get; set; } = string.Empty;

        public string? ProfilioNuotrauka { get; set; } // Base64 string

        [TextLengthValidator(1, 100)]
        public string Miestas { get; set; } = string.Empty;

        [TextLengthValidator(1, 100)]
        public string Gatve { get; set; } = string.Empty;

        [TextLengthValidator(1, 10)]
        public string NamoNumeris { get; set; } = string.Empty;

        public string? ButoNumeris { get; set; } // Nullable
    }

}
