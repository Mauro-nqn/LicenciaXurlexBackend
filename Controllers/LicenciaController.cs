using LicenciaBackend.Data;
using LicenciaBackend.Helpers;
using LicenciaBackend.Models;
using LicenciaBackend.Models.Dtos;
using LicenciaBackend.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace LicenciaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicenciaController : ControllerBase
{
    private readonly ILicenciaRepositorio _repo;
    private readonly IConfiguration _config;

    public LicenciaController(ILicenciaRepositorio repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }


    [HttpGet("estado")]
    public async Task<IActionResult> VerificarLicencia(
    [FromQuery] string claveLicencia,
    [FromQuery] string dispositivoId)
    {
        var licencia = await _repo.VerificarLicenciaAsync(claveLicencia, dispositivoId);

        if (licencia == null)
            return Unauthorized(new { mensaje = "❌ Licencia inválida, desactivada o expirada." });

        return Ok(new
        {
            mensaje = "✅ Licencia válida.",
            licencia.Info,
            licencia.FechaCreacion,
            licencia.ValidoHasta,
            licencia.Activa
        });
    }



    //ANTERIOR

    //[HttpPost("emitir")]
    //public async Task<IActionResult> Emitir([FromBody] LicenciaFirmada model,
    //    [FromHeader(Name = "X-Licencia-Key")] string masterKey)
    //{
    //    var claveEsperada = _config["LicenciaTool:MasterKey"];
    //    if (string.IsNullOrEmpty(masterKey) || masterKey != claveEsperada)
    //        return Unauthorized("Clave inválida.");

    //    // Validar firma
    //    var datos = Convert.FromBase64String(model.DatosBase64);
    //    var firma = Convert.FromBase64String(model.FirmaBase64);

    //    var clavePublica = System.IO.File.ReadAllText("Claves/clave_publica.pem");
    //    using var rsa = RSA.Create();
    //    rsa.ImportFromPem(clavePublica.ToCharArray());

    //    var valida = rsa.VerifyData(datos, firma, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    //    if (!valida)
    //        return BadRequest("Firma inválida.");

    //    var info = JsonSerializer.Deserialize<LicenciaInfo>(Encoding.UTF8.GetString(datos))!;

    //    var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

    //    var licencia = await _repo.GuardarAsync(info, model, ip, "LicenciaTool");

    //    return Ok(new
    //    {
    //        id = licencia.Id,
    //        mensaje = "Licencia registrada correctamente."
    //    });



    //}




    [Authorize(Policy = "LicenciasAdmin")]
    [HttpPost("emitir")]
    public async Task<IActionResult> Emitir([FromBody] LicenciaFirmada model,
    [FromHeader(Name = "X-Licencia-Key")] string? masterKey)
    {
        //  Validar clave maestra
        var claveEsperada = _config["LicenciaTool:MasterKey"];
        if (string.IsNullOrWhiteSpace(masterKey) || masterKey != claveEsperada)
            return Unauthorized("Clave inválida.");

        //  Extraer datos
        byte[] datosConIV, firma;
        try
        {
            datosConIV = Convert.FromBase64String(model.DatosBase64);
            firma = Convert.FromBase64String(model.FirmaBase64);
        }
        catch
        {
            return BadRequest("Error al decodificar base64.");
        }

        //  Verificar firma sobre datos cifrados
        try
        {
            var clavePublica = System.IO.File.ReadAllText("Claves/clave_publica.pem");
            using var rsa = RSA.Create();
            rsa.ImportFromPem(clavePublica.ToCharArray());

            if (!rsa.VerifyData(datosConIV, firma, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                return BadRequest("Firma inválida.");
        }
        catch (Exception ex)
        {
            return BadRequest("Error verificando firma: " + ex.Message);
        }

        //  Descifrar con AES (primeros 16 bytes = IV)
        byte[] aesKey, jsonBytes;
        try
        {
            aesKey = System.IO.File.ReadAllBytes("Claves/aes.key");
            if (aesKey.Length != 16 && aesKey.Length != 24 && aesKey.Length != 32)
                return BadRequest("Tamaño de clave AES inválido.");

            var iv = datosConIV.Take(16).ToArray();
            var datosCifrados = datosConIV.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            jsonBytes = decryptor.TransformFinalBlock(datosCifrados, 0, datosCifrados.Length);
        }
        catch (Exception ex)
        {
            return BadRequest("Error al descifrar con AES: " + ex.Message);
        }

        //  Deserializar JSON
        LicenciaInfo info;
        try
        {
            var json = Encoding.UTF8.GetString(jsonBytes);
            info = JsonSerializer.Deserialize<LicenciaInfo>(json) ?? throw new Exception("LicenciaInfo nulo");
        }
        catch (Exception ex)
        {
            return BadRequest("Error al deserializar JSON: " + ex.Message);
        }

        //  Registrar licencia
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var licencia = await _repo.GuardarAsync(info, model, ip, "LicenciaTool");

        return Ok(new
        {
            id = licencia.Id,
            mensaje = "Licencia registrada correctamente."
        });
    }







    [AllowAnonymous]
    [HttpPost("verificar")]
    public async Task<IActionResult> VerificarLicencia([FromBody] LicenciaVerificacionDto dto)
    {
        var licencia = await _repo.VerificarLicenciaAsync(dto.ClaveLicencia, dto.DispositivoId);

    //    var horaArgentina = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    //TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));
    //    System.Diagnostics.Debug.WriteLine($"✅ Datetime: {horaArgentina} ");


        var utcSimulada = DateTime.UtcNow.AddHours(-5); // 5 horas antes
        var horaArgentina = TimeZoneInfo.ConvertTimeFromUtc(utcSimulada,
            TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time"));

        Console.WriteLine($"🕒 Hora Argentina simulada: {horaArgentina}");

        if (licencia == null || !licencia.Activa || licencia.ValidoHasta < horaArgentina)
            return Unauthorized(new { mensaje = "Licencia inválida o expirada" });

        //return Ok(new LicenciaVerificadaDto
        //{
        //    Clave = licencia.Clave,
        //    DispositivoId = licencia.DispositivoId,
        //    FechaExpiracion = licencia.ValidoHasta,
        //    Activa = licencia.Activa,
        //    MaxUsuariosLocales = licencia.MaxUsuariosLocales
        //});

        //System.Diagnostics.Debug.WriteLine($"✅ Verificada: {licencia.Clave} - UsuariosLocales: {licencia.MaxUsuariosLocales} - Dispositivos: {licencia.MaxDispositivos}");


        return Ok(new LicenciaVerificadaDto
        {
            Clave = licencia.Clave,
            DispositivoId = licencia.DispositivoId,
            FechaExpiracion = licencia.ValidoHasta,
            Activa = licencia.Activa,
            MaxUsuariosLocales = licencia.MaxUsuariosLocales,
            MaxDispositivos = licencia.MaxDispositivos,
            PermitirRemoto = licencia.PermitirRemoto,
            ApiUrl = licencia.ApiUrl
        });
    }





    [AllowAnonymous]
    [HttpPost("verificar-id")]
    public async Task<IActionResult> VerificarLicenciaPorId([FromBody] LicenciaVerificacionDto dto)
    {
        var licencia = await _repo.GetByIdAsync(dto.IdLicencia);

        if (licencia == null)
            return NotFound("Licencia no encontrada.");

        if (!licencia.Activa)
            return Unauthorized("Licencia desactivada.");

        //if (licencia.DispositivoId != dto.DispositivoId)
        //    return Unauthorized("Dispositivo no autorizado.");

        // Comparación sin sensibilidad a mayúsculas/minúsculas
        if (!string.Equals(licencia.DispositivoId, dto.DispositivoId, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Dispositivo no autorizado.");

        if (licencia.ValidoHasta < DateTime.UtcNow)
            return Unauthorized("Licencia expirada.");

        return Ok(new LicenciaVerificadaDto
        {
            Clave = licencia.Clave,
            DispositivoId = licencia.DispositivoId,
            FechaExpiracion = licencia.ValidoHasta,
            Activa = licencia.Activa,
            MaxUsuariosLocales = licencia.MaxUsuariosLocales,
            ApiUrl = licencia.ApiUrl
        });
    }



    [AllowAnonymous]
    [HttpGet("info")]
    public async Task<IActionResult> ObtenerInfoLicencia(
        [FromHeader(Name = "X-Monitor-Key")] string? monitorKey,
        [FromHeader(Name = "X-Licencia-Clave")] string? claveLicencia)


    {

        System.Diagnostics.Debug.WriteLine("📥 [GET /info] llamada recibida");
        System.Diagnostics.Debug.WriteLine($"🔐 Monitor-Key: {monitorKey}");
        System.Diagnostics.Debug.WriteLine($"🔐 Licencia-Clave: {claveLicencia}");

        //const string CLAVE_MONITOR = "iurix-monitor-2024";



        //if (monitorKey != CLAVE_MONITOR)
        //    return Unauthorized("Clave del monitor inválida");


        // ⬇️ leer clave esperada desde configuración
        var claveMonitorEsperada = _config["Monitor:Key"];

        if (string.IsNullOrWhiteSpace(claveMonitorEsperada))
            return StatusCode(500, "Clave del monitor no configurada en el servidor.");

        if (monitorKey != claveMonitorEsperada)
            return Unauthorized("Clave del monitor inválida");


        if (string.IsNullOrEmpty(claveLicencia))
            return BadRequest("Debe enviar la clave de licencia en el header 'X-Licencia-Clave'.");

        var licencia = await _repo.GetByClaveAsync(claveLicencia);

        if (licencia == null)
            return NotFound("Licencia no registrada");

        var estado = LicenciaHelper.CalcularEstado(licencia.ValidoHasta);

        var dto = new LicenciaInfoMonitorDto
        {
            EstudioNombre = licencia.Info.EstudioNombre,
            CUIT = licencia.Info.CUIT,
            ValidoHasta = licencia.ValidoHasta,
            Estado = estado,
            ApiUrl = licencia.ApiUrl // ✅ Se incluye la IP pública si fue registrada
        };

        //System.Diagnostics.Debug.WriteLine($"✅ Licencia OK. Estudio: {dto.EstudioNombre}, CUIT: {dto.CUIT}");
        return Ok(dto);
    }


    [Authorize(Policy = "LicenciasAdmin")]
    [HttpPut("desactivar")]
    public async Task<IActionResult> DesactivarLicencia(
    [FromQuery] string dispositivoId,
    [FromHeader(Name = "X-Licencia-Key")] string? masterKey)
    {
        var claveEsperada = _config["LicenciaTool:MasterKey"];
        if (string.IsNullOrEmpty(masterKey) || masterKey != claveEsperada)
            return Unauthorized(new { mensaje = "No autorizado para desactivar licencias." });

        var licencia = await _repo.GetByDispositivoAsync(dispositivoId);
        if (licencia == null)
            return NotFound(new { mensaje = "No se encontró licencia activa para este dispositivo." });

        licencia.Activa = false;
        await _repo.ActualizarLicenciaAsync(licencia);

        return Ok(new { mensaje = "Licencia desactivada correctamente." });
    }


    //[HttpDelete("{claveLicencia}")]
    //public async Task<IActionResult> Eliminar(string claveLicencia)
    //{
    //    var lic = await _repo.GetByClaveAsync(claveLicencia);
    //    if (lic == null)
    //        return NotFound(new { mensaje = "Licencia no encontrada." });

    //    await _repo.EliminarAsync(lic);
    //    return Ok(new { mensaje = "Licencia eliminada correctamente." });
    //}



    [Authorize(Policy = "LicenciasAdmin")]
    [HttpDelete("por-id/{id}")]
    public async Task<IActionResult> EliminarPorId(int id)
    {
        var licencia = await _repo.GetByIdAsync(id);
        if (licencia == null)
            return NotFound(new { mensaje = "Licencia no encontrada." });

        await _repo.EliminarAsync(licencia);
        return Ok(new { mensaje = "Licencia eliminada correctamente." });
    }






    //[HttpPost("actualizar-ip")]
    //public async Task<IActionResult> ActualizarIp([FromBody] ActualizarIpDto dto)
    //{
    //    if (string.IsNullOrWhiteSpace(dto.Clave) || string.IsNullOrWhiteSpace(dto.Ip))
    //        return BadRequest("Clave e IP son requeridas.");

    //    var licencia = await _repo.GetByClaveAsync(dto.Clave);

    //    if (licencia == null)
    //        return NotFound("Licencia no encontrada.");

    //    // Actualizamos la IP en el campo ApiUrl (u otro que uses para registrar IP pública)
    //    licencia.ApiUrl = dto.Ip;
    //    await _repo.ActualizarLicenciaAsync(licencia);

    //    return Ok(new { mensaje = "IP pública actualizada correctamente." });
    //}




    [HttpPost("actualizar-ip")]
    public async Task<IActionResult> ActualizarIp([FromBody] ActualizarIpDto dto)
    {
        System.Diagnostics.Debug.WriteLine($"📥 IP recibida: {dto.Ip}, Clave: {dto.Clave}");

        if (string.IsNullOrWhiteSpace(dto.Clave) || string.IsNullOrWhiteSpace(dto.Ip))
        {
            System.Diagnostics.Debug.WriteLine("❌ Clave o IP vacía.");
            return BadRequest("Clave e IP son requeridas.");
        }

        var licencia = await _repo.GetByClaveAsync(dto.Clave);
        if (licencia == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ Licencia no encontrada.");
            return NotFound("Licencia no encontrada.");
        }

        System.Diagnostics.Debug.WriteLine($"🔧 Licencia encontrada. IP anterior: {licencia.ApiUrl}");

        //licencia.ApiUrl = dto.Ip;
        licencia.ApiUrl = dto.Ip;
        licencia.Puerto = dto.Puerto;
        licencia.IpLocal = dto.IpLocal;
        await _repo.ActualizarLicenciaAsync(licencia);

        System.Diagnostics.Debug.WriteLine($"✅ IP Pública actualizada en objeto: {licencia.ApiUrl}, IP Local actualizada: {licencia.IpLocal} y puerto: {licencia.Puerto}");

        return Ok(new { mensaje = "IP pública, Local y Puerto actualizados correctamente." });
    }


    ////devolvemos la clave a la aplicacion del cliente front
    //// GET: api/licencia/clave/ABC123
    //[HttpGet("clave/{clave}")]
    //public async Task<ActionResult<LicenciaRegistrada>> GetByClave(string clave)
    //{
    //    var licencia = await _repo.GetByClaveAsync(clave);

    //    if (licencia == null)
    //        return NotFound();

    //    return new LicenciaRegistrada
    //    {
    //        Clave = licencia.Clave,
    //        DispositivoId = licencia.DispositivoId,
    //        ApiUrl = licencia.ApiUrl // Asegurate de tener esto en la entidad
    //        //UltimaIp = licencia.UltimaIp
    //    };
    //}

    [AllowAnonymous]
    [HttpGet("clave/{clave}")]
    public async Task<ActionResult<LicenciaInfoMonitorDto>> GetByClave(string clave)
    {
        var licencia = await _repo.GetByClaveAsync(clave);

        if (licencia == null)
            return Ok(new LicenciaInfoMonitorDto
            {
                EstudioNombre = "(No registrada)",
                CUIT = "",
                Estado = "NoRegistrada",
                ValidoHasta = DateTime.MinValue,
                ApiUrl = null
            });

        var estado = LicenciaHelper.CalcularEstado(licencia.ValidoHasta);

        return Ok(new LicenciaInfoMonitorDto
        {
            EstudioNombre = licencia.Info.EstudioNombre,
            CUIT = licencia.Info.CUIT,
            ValidoHasta = licencia.Info.ValidoHasta,
            Estado = estado,
            ApiUrl = licencia.ApiUrl
        });
    }




    [HttpGet("clave/{clave}/dispositivo/{dispositivoId}")]
    public async Task<ActionResult<LicenciaInfo>> GetByClaveYDispositivo(string clave, string dispositivoId)
    {
        var licencia = await _repo.GetByClaveYDispositivoAsync(clave, dispositivoId);

        if (licencia == null)
            return NotFound();

        return new LicenciaInfo
        {

            EstudioNombre = licencia.Info.EstudioNombre,
            CUIT = licencia.Info.CUIT,
            Clave = licencia.Clave,
            DispositivoId = licencia.DispositivoId,
            ValidoDesde = licencia.Info.ValidoDesde,
            ValidoHasta = licencia.Info.ValidoHasta,
            MaxUsuariosLocales = licencia.MaxUsuariosLocales,
            MaxUsuariosRemotos = licencia.Info.MaxUsuariosRemotos,
            PermitirRemoto = licencia.Info.PermitirRemoto,
            Activa = licencia.Activa,
            ApiUrl = licencia.ApiUrl ?? ""
        };
    }



    //[Authorize(Policy = "LicenciasAdmin")]
    [AllowAnonymous]
    [HttpGet("obtener/{id}")]
    public async Task<IActionResult> ObtenerLicenciaPorId/*(int id)*/
        (
    [FromRoute] int id,
    [FromHeader(Name = "X-Licencia-Key")] string? masterKey)
    {         
        var claveEsperada = _config["LicenciaTool:MasterKey"];
        if (string.IsNullOrEmpty(masterKey) || masterKey != claveEsperada)
            return Unauthorized(new { mensaje = "No autorizado obtener licencia." });
    
        var licencia = await _repo.GetByIdAsync(id);
        if (licencia == null)
            return NotFound("Licencia no encontrada");

        var estado = LicenciaHelper.CalcularEstado(licencia.Info.ValidoHasta);

        return Ok(new LicenciaInfoMonitorDto
        {
            IdLicencia = licencia.Id,
            EstudioNombre = licencia.Info.EstudioNombre,
            CUIT = licencia.Info.CUIT,
            Clave = licencia.Info.Clave,
            DispositivoId = licencia.Info.DispositivoId,
            //Modo = licencia.Info.Modo,
            //ValidoDesde = licencia.Info.ValidoDesde,
            ValidoHasta = licencia.Info.ValidoHasta,
            ApiUrl = licencia.ApiUrl,
            IpLocal = licencia.IpLocal,
            Puerto = licencia.Puerto,
            //MaxUsuariosLocales = licencia.Info.MaxUsuariosLocales,
            //MaxUsuariosRemotos = licencia.Info.MaxUsuariosRemotos,
            //PermitirRemoto = licencia.Info.PermitirRemoto,
            //MaxDispositivos = licencia.Info.MaxDispositivos,
            //Notas = licencia.Info.Notas,
            Estado = estado
        });
    }




}