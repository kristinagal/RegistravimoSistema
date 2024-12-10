using System;

namespace RegistravimoSistema.Entities
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Miestas { get; set; } = string.Empty;
        public string Gatve { get; set; } = string.Empty;
        public string NamoNumeris { get; set; } = string.Empty;
        public string? ButoNumeris { get; set; }

        // Navigation Property
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }

}
