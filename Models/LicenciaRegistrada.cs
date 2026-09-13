namespace LicenciaBackend.Models;

public class LicenciaRegistrada
{
    public int Id { get; set; }    
    public LicenciaInfo Info { get; set; } = new();
    public LicenciaFirmada Firma { get; set; } = new();
    public string? IpOrigen { get; set; }
    public string? Origen { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime ValidoHasta { get; set; } = DateTime.UtcNow.AddYears(1);

    public string Clave { get; set; } = "";
    public bool Activa { get; set; }

    public string DispositivoId { get; set; } = string.Empty;
    public int MaxUsuariosLocales { get; set; } = 1;
    public int MaxDispositivos { get; set; } = 1;

    public bool PermitirRemoto { get; set; } = false;

    public string? ApiUrl { get; set; }

    public int Puerto { get; set; }

    public string? IpLocal { get; set; }  // NUEVO: la IP local interna (LAN) del servidor

}
