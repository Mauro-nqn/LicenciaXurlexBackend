namespace LicenciaBackend.Models.Dtos
{
    public class ActualizarIpDto
    {
        public string Clave { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;

        public string? IpLocal { get; set; }

        public int Puerto { get; set; }


    }
}
