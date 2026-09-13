namespace LicenciaBackend.Services
{
    //public class ConfigService
    //{
    //    public int Puerto { get; set; } = 7140;

    //    public bool UsaHttpsLicenciaBackend { get; set; } = false;



    //    public void LeerConfiguracion()
    //    {
    //        var ruta = Path.Combine(AppContext.BaseDirectory, "Config/config.ini");
    //        if (!File.Exists(ruta)) return;

    //        var secciones = new Dictionary<string, Dictionary<string, string>>();
    //        string? seccionActual = null;

    //        foreach (var linea in File.ReadAllLines(ruta))
    //        {
    //            var trimmed = linea.Trim();

    //            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";"))
    //                continue;

    //            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
    //            {
    //                seccionActual = trimmed[1..^1];
    //                secciones[seccionActual] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    //            }
    //            else if (seccionActual != null && trimmed.Contains('='))
    //            {
    //                var partes = trimmed.Split('=', 2);
    //                var clave = partes[0].Trim();
    //                var valor = partes[1].Trim();
    //                secciones[seccionActual][clave] = valor;
    //            }
    //        }

    //        if (secciones.TryGetValue("Servidor", out var servidor))
    //        {
    //            if (servidor.TryGetValue("puerto", out var puertoStr) && int.TryParse(puertoStr, out var puerto))
    //                Puerto = puerto;
    //        }


    //    }
    //}

    public sealed class ConfigService
    {
        public int Puerto { get; private set; } = 7140;
        public bool UsaHttps { get; private set; } = false;

        public string SelfBaseUrl => BuildUrl("localhost", Puerto, UsaHttps);

        public void LeerConfiguracion()
        {
            var ruta = GetIniPath();
            var ini = ParseIni(ruta);

            if (ini.TryGetValue("Servidor", out var srv))
            {
                if (srv.TryGetValue("puerto", out var pStr) && int.TryParse(pStr, out var p))
                    Puerto = p;

                if (srv.TryGetValue("https", out var hStr))
                    UsaHttps = hStr.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);

                // En Azure, si https=true y puerto no vino o vino 80, asumimos 443
                if (IsAzure() && UsaHttps && (Puerto == 0 || Puerto == 80))
                    Puerto = 443;
            }
        }

        // ===== Helpers =====
        private static bool IsAzure() =>
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")) ||
            string.Equals(Environment.GetEnvironmentVariable("IURIX_MODE"), "Azure", StringComparison.OrdinalIgnoreCase);

        private static string GetIniPath()
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Config");
            Directory.CreateDirectory(folder);
            var azure = Path.Combine(folder, "config.azure.ini");
            var local = Path.Combine(folder, "config.ini");
            return IsAzure() && File.Exists(azure) ? azure : local;
        }

        private static string BuildUrl(string host, int port, bool https)
        {
            var scheme = https ? "https" : "http";
            var omit = (https && port == 443) || (!https && port == 80);
            return omit ? $"{scheme}://{host}" : $"{scheme}://{host}:{port}";
        }

        private static Dictionary<string, Dictionary<string, string>> ParseIni(string ruta)
        {
            var map = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(ruta)) return map;

            string? current = null;
            foreach (var raw in File.ReadAllLines(ruta))
            {
                var s = raw.Trim();
                if (string.IsNullOrWhiteSpace(s)) continue;
                if (s.StartsWith(";") || s.StartsWith("#") || s.StartsWith("//")) continue;

                if (s.StartsWith("[") && s.EndsWith("]"))
                {
                    current = s[1..^1].Trim();
                    map[current] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                var eq = s.IndexOf('=');
                if (eq <= 0 || current == null) continue;

                var key = s[..eq].Trim();
                var val = s[(eq + 1)..].Trim();
                map[current][key] = val;
            }
            return map;
        }
    }

}
