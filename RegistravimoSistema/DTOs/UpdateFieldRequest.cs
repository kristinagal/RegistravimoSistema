using RegistravimoSistema.Validations;
using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.DTOs
{
    public class UpdateVardasRequest
    {
        /// <summary>
        /// Vardas
        /// </summary>
        [Required, TextLengthValidator(2, 50)]
        public string Vardas { get; set; } = string.Empty;
    }

    public class UpdatePavardeRequest
    {
        /// <summary>
        /// Pavardė
        /// </summary>
        [Required, TextLengthValidator(2, 50)]
        public string Pavarde { get; set; } = string.Empty;
    }

    public class UpdateAsmensKodasRequest
    {
        /// <summary>
        /// Asmens kodas
        /// </summary>
        [Required, AsmensKodasValidator]
        public string AsmensKodas { get; set; } = string.Empty;
    }

    public class UpdateTelefonoNumerisRequest
    {
        /// <summary>
        /// Telefono numeris
        /// </summary>
        [Required, TelefonoNumerisValidator]
        public string TelefonoNumeris { get; set; } = string.Empty;
    }

    public class UpdateElPastasRequest
    {
        /// <summary>
        /// Elektroninis paštas
        /// </summary>
        [Required, EmailValidator]
        public string ElPastas { get; set; } = string.Empty;
    }

    public class UpdateProfilioNuotraukaRequest
    {
        /// <summary>
        /// Profilio nuotrauka
        /// </summary>
        [Required]
        public string ProfilioNuotrauka { get; set; } = string.Empty; // Base64 encoded string
    }

    public class UpdateMiestasRequest
    {
        /// <summary>
        /// Miestas
        /// </summary>
        [Required, TextLengthValidator(1, 100)]
        public string Miestas { get; set; } = string.Empty;
    }

    public class UpdateGatveRequest
    {
        /// <summary>
        /// Gatvė
        /// </summary>
        [Required, TextLengthValidator(1, 100)]
        public string Gatve { get; set; } = string.Empty;
    }

    public class UpdateNamoNumerisRequest
    {
        /// <summary>
        /// Namo numeris
        /// </summary>
        [Required, TextLengthValidator(1, 10)]
        public string NamoNumeris { get; set; } = string.Empty;
    }

    public class UpdateButoNumerisRequest
    {
        /// <summary>
        /// Buto numeris
        /// </summary>
        [TextLengthValidator(1, 10)]
        public string ButoNumeris { get; set; } = string.Empty;
    }
}
