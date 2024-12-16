using RegistravimoSistema.Validations;
using System.ComponentModel.DataAnnotations;

public class PersonRequest
{
    [Required, TextLengthValidator(2, 50)]
    public string Vardas { get; set; } = string.Empty;

    [Required, TextLengthValidator(2, 50)]
    public string Pavarde { get; set; } = string.Empty;

    [Required, AsmensKodasValidator]
    public string AsmensKodas { get; set; } = string.Empty;

    [Required, TelefonoNumerisValidator]
    public string TelefonoNumeris { get; set; } = string.Empty;

    [Required, EmailValidator]
    public string ElPastas { get; set; } = string.Empty;

    [Required]
    public string ProfilioNuotrauka { get; set; } = string.Empty;

    [Required, TextLengthValidator(1, 100)]
    public string Miestas { get; set; } = string.Empty;

    [Required, TextLengthValidator(1, 100)]
    public string Gatve { get; set; } = string.Empty;

    [Required, TextLengthValidator(1, 10)]
    public string NamoNumeris { get; set; } = string.Empty;

    public string? ButoNumeris { get; set; } // Nullable
}
