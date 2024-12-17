namespace RegistravimoSistema.DTOs
{
    public class AddressResponse
    {
        /// <summary>
        /// Addresas: miestas
        /// </summary>
        public string Miestas { get; set; } = string.Empty;

        /// <summary>
        /// Addresas: gatve
        /// </summary>
        public string Gatve { get; set; } = string.Empty;

        /// <summary>
        /// Addresas: namo numeris
        /// </summary>
        public string NamoNumeris { get; set; } = string.Empty;

        /// <summary>
        /// Addresas: buto numeris
        /// </summary>
        public string ButoNumeris { get; set; } = string.Empty;
    }
}
