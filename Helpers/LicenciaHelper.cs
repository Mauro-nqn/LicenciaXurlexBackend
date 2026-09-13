namespace LicenciaBackend.Helpers
{
    public static class LicenciaHelper
    {
        public static string CalcularEstado(DateTime validoHasta)
        {
            var dias = (validoHasta.Date - DateTime.UtcNow.Date).TotalDays;
            if (dias < 0) return "Vencida";
            if (dias <= 15) return "Por vencer";
            return "Vigente";
        }
    }
}
