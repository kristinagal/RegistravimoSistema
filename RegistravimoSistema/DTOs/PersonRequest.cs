using RegistravimoSistema.Validations;
using System.ComponentModel.DataAnnotations;

public class PersonRequest
{
    /// <summary>
    /// Vardas
    /// </summary>
    [Required, TextLengthValidator(2, 50)]
    public string Vardas { get; set; } = string.Empty;

    /// <summary>
    /// Pavardė
    /// </summary>
    [Required, TextLengthValidator(2, 50)]
    public string Pavarde { get; set; } = string.Empty;

    /// <summary>
    /// Asmens kodas
    /// </summary>
    [Required, AsmensKodasValidator]
    public string AsmensKodas { get; set; } = string.Empty;

    /// <summary>
    /// Telefono numeris
    /// </summary>
    [Required, TelefonoNumerisValidator]
    public string TelefonoNumeris { get; set; } = string.Empty;

    /// <summary>
    /// Elektroninis paštas
    /// </summary>
    [Required, EmailValidator]
    public string ElPastas { get; set; } = string.Empty;

    /// <summary>
    /// Profilio nuotrauka
    /// </summary>
    [Required]
    public string ProfilioNuotrauka { get; set; } = string.Empty;

    /// <summary>
    /// Addresas: miestas
    /// </summary>
    [Required, TextLengthValidator(1, 100)]
    public string Miestas { get; set; } = string.Empty;

    /// <summary>
    /// Addresas: gatve
    /// </summary>
    [Required, TextLengthValidator(1, 100)]
    public string Gatve { get; set; } = string.Empty;

    /// <summary>
    /// Addresas: namo numeris
    /// </summary>
    [Required, TextLengthValidator(1, 10)]
    public string NamoNumeris { get; set; } = string.Empty;

    /// <summary>
    /// Addresas: buto numeris
    /// </summary>
    [Required, TextLengthValidator(1, 10)]
    public string ButoNumeris { get; set; } = string.Empty;
}
