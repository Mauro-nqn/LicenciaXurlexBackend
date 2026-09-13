using LicenciaBackend.Data;
using LicenciaBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenciaBackend.Repositorios;

public class LicenciaRepositorio : ILicenciaRepositorio
{
    private readonly LicenciaDbContext _context;

    public LicenciaRepositorio(LicenciaDbContext context)
    {
        _context = context;
    }

    //public async Task<LicenciaRegistrada> GuardarAsync(LicenciaInfo info, LicenciaFirmada firma, string? ip, string origen)
    //{
    //    var lic = new LicenciaRegistrada
    //    {
    //        Clave = info.Clave,
    //        DispositivoId = info.DispositivoId,
    //        Info = info,
    //        Firma = firma,
    //        IpOrigen = ip,
    //        Origen = origen,
    //        Activa = true

    //    };

    //    _context.Licencias.Add(lic);
    //    await _context.SaveChangesAsync();

    //    return lic;
    //}



    public async Task<LicenciaRegistrada> GuardarAsync(LicenciaInfo info, LicenciaFirmada firma, string? ip, string? origen)
    {
        var licencia = new LicenciaRegistrada
        {
            Clave = info.Clave,
            DispositivoId = info.DispositivoId,
            Activa = info.Activa,
            ValidoHasta = info.ValidoHasta,
            MaxUsuariosLocales = info.MaxUsuariosLocales,
            MaxDispositivos = info.MaxDispositivos,
            PermitirRemoto = info.PermitirRemoto,
            ApiUrl = info.ApiUrl,
            Info = info,
            Firma = firma,
            IpOrigen = ip,
            Origen = origen
        };

        _context.Licencias.Add(licencia);
        await _context.SaveChangesAsync();
        return licencia;
    }

    public async Task<List<LicenciaRegistrada>> ListarAsync()
    {
        return await _context.Licencias.ToListAsync();
    }


    public async Task<LicenciaRegistrada?> GetByDispositivoAsync(string dispositivoId)
    {
        return await _context.Licencias
            .FirstOrDefaultAsync(l => l.DispositivoId == dispositivoId && l.Activa);
    }



    public async Task ActualizarLicenciaAsync(LicenciaRegistrada licencia)
    {
        _context.Licencias.Update(licencia);
        await _context.SaveChangesAsync();
    }

    //public async Task<LicenciaRegistrada?> VerificarLicenciaAsync(string claveLicencia, string dispositivoId)
    //{
    //    var clave = claveLicencia.Trim().ToLower();
    //    var dispositivo = dispositivoId.Trim().ToLower();

    //    return await _context.Licencias
    //        .FirstOrDefaultAsync(l =>
    //            l.Clave.Trim().ToLower() == clave &&
    //            l.DispositivoId.Trim().ToLower() == dispositivo &&
    //            l.Activa &&
    //            l.ValidoHasta > DateTime.UtcNow);
    //}



    public async Task<LicenciaRegistrada?> VerificarLicenciaAsync(string claveLicencia, string dispositivoId)
    {
        var clave = claveLicencia.Trim().ToLower();
        var dispositivo = dispositivoId.Trim().ToLower();

        var utcSimulada = DateTime.UtcNow.AddHours(-5); // 5 horas antes
        var horaArgentina = TimeZoneInfo.ConvertTimeFromUtc(utcSimulada,
            TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));

        Console.WriteLine($"🕒 Hora Argentina simulada: {horaArgentina}");

        

        System.Diagnostics.Debug.WriteLine($"🔍 Buscando licencia con:");
        System.Diagnostics.Debug.WriteLine($"🔑 Clave: {clave}");
        System.Diagnostics.Debug.WriteLine($"💻 Dispositivo: {dispositivo}");
        System.Diagnostics.Debug.WriteLine($"🕒 Fecha actual (UTC): {horaArgentina}");

        var todas = await _context.Licencias.ToListAsync();

        foreach (var lic in todas)
        {
            System.Diagnostics.Debug.WriteLine("–––––––––––––––––––––––––––––––");
            System.Diagnostics.Debug.WriteLine($"➡ Licencia ID: {lic.Id}");
            System.Diagnostics.Debug.WriteLine($"   Clave BD: {lic.Clave}");
            System.Diagnostics.Debug.WriteLine($"   DispositivoId BD: {lic.DispositivoId}");
            System.Diagnostics.Debug.WriteLine($"   Activa: {lic.Activa}");
            System.Diagnostics.Debug.WriteLine($"   ValidoHasta: {lic.ValidoHasta} (UTC)");
            System.Diagnostics.Debug.WriteLine($"   Coincide Clave: {lic.Clave.Trim().ToLower() == clave}");
            System.Diagnostics.Debug.WriteLine($"   Coincide Dispositivo: {lic.DispositivoId.Trim().ToLower() == dispositivo}");
            System.Diagnostics.Debug.WriteLine($"   Fecha válida: {lic.ValidoHasta > horaArgentina}");
            System.Diagnostics.Debug.WriteLine($"   Estado final: {lic.Clave.Trim().ToLower() == clave && lic.DispositivoId.Trim().ToLower() == dispositivo && lic.Activa && lic.ValidoHasta > horaArgentina}");
        }

        var licencia = todas.FirstOrDefault(l =>
            l.Clave.Trim().ToLower() == clave &&
            l.DispositivoId.Trim().ToLower() == dispositivo &&
            l.Activa &&
            l.ValidoHasta > horaArgentina);

        return licencia;
    }


    




    public async Task<LicenciaRegistrada?> GetByClaveAsync(string clave)
    {
        return await _context.Licencias.FirstOrDefaultAsync(l => l.Clave == clave);
    }

    

    public async Task<LicenciaRegistrada?> GetByIdAsync(int id)
    {
        return await _context.Licencias.FindAsync(id);
    }

    //public async Task<LicenciaRegistrada?> GetByIdAsync(int id)
    //{
    //    return await _context.Licencias.FirstOrDefaultAsync(l => l.Id == id);
    //}


    public async Task<LicenciaRegistrada?> GetByClaveYDispositivoAsync(string clave, string dispositivoId)
    {
        return await _context.Licencias
            .FirstOrDefaultAsync(l => l.Clave == clave && l.DispositivoId == dispositivoId);
    }



    //public async Task EliminarAsync(LicenciaRegistrada licencia)
    //{
    //    _context.Licencias.Remove(licencia);
    //    await _context.SaveChangesAsync();
    //}


    public async Task EliminarAsync(LicenciaRegistrada licencia)
    {
        _context.Licencias.Remove(licencia);
        await _context.SaveChangesAsync();
    }




}
