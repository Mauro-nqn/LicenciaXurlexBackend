namespace LicenciaBackend.Models;

public class LicenciaInfo
{
    public int Id { get; set; } = 1;
    public string EstudioNombre { get; set; } = "";
    public string CUIT { get; set; } = "";
    public string Clave { get; set; } = "";
    public string DispositivoId { get; set; } = "";
    public string Modo { get; set; } = "Servidor";
    public DateTime ValidoDesde { get; set; } = DateTime.UtcNow;
    public DateTime ValidoHasta { get; set; } = DateTime.UtcNow.AddYears(1);
    public string? ApiUrl { get; set; }
    public int MaxUsuariosLocales { get; set; } = 1;
    public int MaxUsuariosRemotos { get; set; } = 1;
    public bool PermitirRemoto { get; set; } = false;
    public int MaxDispositivos { get; set; } = 1;
    public string? Notas { get; set; }
    public bool Activa { get; set; }
}
