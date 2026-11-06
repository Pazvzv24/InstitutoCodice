using System;
using System.Linq;
using System.Text;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Utilidades para exportar informes: nombres de archivo seguros.
    /// </summary>
    public static class FormatoInforme
    {
        // Caracteres no permitidos en nombres de archivo de Windows
        private static readonly char[] NoPermitidos = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        /// <summary>
        /// Sugiere un nombre de archivo, por ejemplo: Informe_Curso_POO101_20261106_1430.pdf
        /// Reemplaza espacios y caracteres no permitidos para evitar rutas inválidas.
        /// </summary>
        public static string NombreArchivo(string tipo, string referencia, DateTime fecha, string extension)
        {
            return "Informe_" + Limpiar(tipo) + "_" + Limpiar(referencia) + "_"
                 + fecha.ToString("yyyyMMdd_HHmm") + "." + extension.TrimStart('.');
        }

        public static string Limpiar(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "SinNombre";

            var sb = new StringBuilder();
            foreach (char c in texto.Trim())
                sb.Append(char.IsControl(c) || char.IsWhiteSpace(c) || NoPermitidos.Contains(c) ? '_' : c);

            string resultado = sb.ToString();
            while (resultado.Contains("__"))
                resultado = resultado.Replace("__", "_");

            return resultado.Trim('_', '.');
        }
    }
}