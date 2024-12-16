using System;
using System.ComponentModel.DataAnnotations;

namespace RegistravimoSistema.Entities
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string Miestas { get; set; } = string.Empty;
        [Required] public string Gatve { get; set; } = string.Empty;
        [Required] public string NamoNumeris { get; set; } = string.Empty;
        [Required] public string ButoNumeris { get; set; } = string.Empty;

        // Navigation Property
        [Required] public Guid PersonId { get; set; }
        [Required] public Person Person { get; set; } = null!;
    }

}
