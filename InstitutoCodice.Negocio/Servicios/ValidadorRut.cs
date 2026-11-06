using System.Linq;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Proporciona validación y cálculo para RUT chilenos.
    /// </summary>
    public static class ValidadorRut
    {
        /// <summary>
        /// Normaliza el RUT eliminando formato previo y aplicando guion estándar.
        /// </summary>
        public static string Normalizar(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut))
                return "";

            string limpio = rut.Replace(".", "").Replace(" ", "").Replace("-", "").Trim().ToUpper();
            if (limpio.Length < 2)
                return limpio;

            return limpio.Substring(0, limpio.Length - 1) + "-" + limpio.Substring(limpio.Length - 1);
        }

        public static bool EsValido(string rut)
        {
            string normalizado = Normalizar(rut);
            int guion = normalizado.IndexOf('-');
            if (guion < 1)
                return false;

            string cuerpo = normalizado.Substring(0, guion);
            string dv = normalizado.Substring(guion + 1);

            if (cuerpo.Length < 7 || cuerpo.Length > 8 || !cuerpo.All(char.IsDigit) || dv.Length != 1)
                return false;

            return dv == CalcularDigitoVerificador(cuerpo);
        }

        public static string CalcularDigitoVerificador(string cuerpo)
        {
            int suma = 0;
            int multiplicador = 2;

            for (int i = cuerpo.Length - 1; i >= 0; i--)
            {
                suma += (cuerpo[i] - '0') * multiplicador;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
            }

            int resultado = 11 - (suma % 11);
            if (resultado == 11) return "0";
            if (resultado == 10) return "K";
            return resultado.ToString();
        }
    }
}
