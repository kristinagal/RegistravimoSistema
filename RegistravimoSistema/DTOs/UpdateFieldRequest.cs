using RegistravimoSistema.Validations;
using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.DTOs
{
    public class UpdateVardasRequest
    {
        [Required, TextLengthValidator(2, 50)]
        public string Vardas { get; set; } = string.Empty;
    }

    public class UpdatePavardeRequest
    {
        [Required, TextLengthValidator(2, 50)]
        public string Pavarde { get; set; } = string.Empty;
    }

    public class UpdateAsmensKodasRequest
    {
        [Required, AsmensKodasValidator]
        public string AsmensKodas { get; set; } = string.Empty;
    }

    public class UpdateTelefonoNumerisRequest
    {
        [Required, TelefonoNumerisValidator]
        public string TelefonoNumeris { get; set; } = string.Empty;
    }

    public class UpdateElPastasRequest
    {
        [Required, EmailValidator]
        public string ElPastas { get; set; } = string.Empty;
    }

    public class UpdateProfilioNuotraukaRequest
    {
        [Required]
        public string ProfilioNuotrauka { get; set; } = string.Empty; // Base64 encoded string
    }

    public class UpdateMiestasRequest
    {
        [Required, TextLengthValidator(1, 100)]
        public string Miestas { get; set; } = string.Empty;
    }

    public class UpdateGatveRequest
    {
        [Required, TextLengthValidator(1, 100)]
        public string Gatve { get; set; } = string.Empty;
    }

    public class UpdateNamoNumerisRequest
    {
        [Required, TextLengthValidator(1, 10)]
        public string NamoNumeris { get; set; } = string.Empty;
    }

    public class UpdateButoNumerisRequest
    {
        [TextLengthValidator(1, 10)]
        public string ButoNumeris { get; set; } = string.Empty;
    }
}
