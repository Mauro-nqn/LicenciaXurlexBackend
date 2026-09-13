namespace LicenciaBackend.Models.Dtos
{
    public class LicenciaVerificadaDto
    {
        public string Clave { get; set; } = string.Empty;
        public string DispositivoId { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Activa { get; set; }
        public int MaxUsuariosLocales { get; set; }
        public bool PermitirRemoto { get; set; } = false;
        public int MaxDispositivos { get; set; } = 1;

        public string? ApiUrl { get; set; }
    }
}
