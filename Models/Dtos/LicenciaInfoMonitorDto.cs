namespace LicenciaBackend.Models.Dtos
{
    public class LicenciaInfoMonitorDto
    {
        public int IdLicencia { get; set; } = 1;
        public string EstudioNombre { get; set; } = "";
        public string DispositivoId { get; set; } = "";

        public string Clave { get; set; } = "";
        public string CUIT { get; set; } = "";
        public DateTime ValidoHasta { get; set; }
        public string Estado { get; set; } = "";

        // ✅ Nueva propiedad para mostrar la IP pública (ApiUrl)
        public string? ApiUrl { get; set; }

        public string? IpLocal { get; set; }
        public int Puerto { get; set; }
    }
}
