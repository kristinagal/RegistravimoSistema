namespace RegistravimoSistema.DTOs
{
    public class PersonResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Vardas
        /// </summary>
        public string Vardas { get; set; } = string.Empty;

        /// <summary>
        /// Pavardė
        /// </summary>
        public string Pavarde { get; set; } = string.Empty;

        /// <summary>
        /// Asmens kodas
        /// </summary>
        public string AsmensKodas { get; set; } = string.Empty;

        /// <summary>
        /// Telefono numeris
        /// </summary>
        public string TelefonoNumeris { get; set; } = string.Empty;

        /// <summary>
        /// Elektroninis paštas
        /// </summary>
        public string ElPastas { get; set; } = string.Empty;

        /// <summary>
        /// Profilio nuotrauka
        /// </summary>
        public string? ProfilioNuotrauka { get; set; } // Base64 encoded string

        /// <summary>
        /// Addresas
        /// </summary>
        public AddressResponse Address { get; set; } = null!;
    }
}
