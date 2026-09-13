using LicenciaBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace LicenciaBackend.Repositorios;

public interface ILicenciaRepositorio
{
    Task<LicenciaRegistrada> GuardarAsync(LicenciaInfo info, LicenciaFirmada firma, string? ip, string origen);
    Task<List<LicenciaRegistrada>> ListarAsync();

    Task<LicenciaRegistrada?> VerificarLicenciaAsync(string claveLicencia, string dispositivoId);

    Task<LicenciaRegistrada?> GetByDispositivoAsync(string dispositivoId); // 🔸 nuevo
    Task ActualizarLicenciaAsync(LicenciaRegistrada licencia);             // 🔸 nuevo

    Task<LicenciaRegistrada?> GetByIdAsync(int id);

    Task<LicenciaRegistrada?> GetByClaveAsync(string clave);


    

    Task<LicenciaRegistrada?> GetByClaveYDispositivoAsync(string clave, string dispositivoId);


    Task EliminarAsync(LicenciaRegistrada licencia);
}
