using System;
using System.Collections.Generic;
using System.Linq;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Reglas mínimas para contraseñas nuevas (RNF-03).
    ///
    /// - Entre 8 y 128 caracteres (el máximo evita abusos con textos enormes).
    /// - Al menos una mayúscula, una minúscula y un número.
    /// - No puede contener el nombre de usuario.
    /// - Debe ser distinta de la contraseña actual.
    /// </summary>
    public static class PoliticaContrasena
    {
        public const int LargoMinimo = 8;
        public const int LargoMaximo = 128;

        /// <summary>
        /// Texto con las reglas, para mostrar al usuario.
        /// </summary>
        public static readonly string Descripcion =
            $"Mínimo {LargoMinimo} caracteres, con al menos una mayúscula, una minúscula y un número. " +
            "No puede contener su nombre de usuario.";

        /// <summary>
        /// Devuelve la lista de reglas que la contraseña no cumple. Lista vacía = contraseña válida.
        /// </summary>
        public static List<string> Validar(string clave, string nombreUsuario, string? claveActual = null)
        {
            var errores = new List<string>();
            clave ??= string.Empty;

            if (clave.Length < LargoMinimo)
                errores.Add($"Debe tener al menos {LargoMinimo} caracteres.");
            if (clave.Length > LargoMaximo)
                errores.Add($"No puede superar los {LargoMaximo} caracteres.");
            if (!clave.Any(char.IsUpper))
                errores.Add("Debe incluir al menos una letra mayúscula.");
            if (!clave.Any(char.IsLower))
                errores.Add("Debe incluir al menos una letra minúscula.");
            if (!clave.Any(char.IsDigit))
                errores.Add("Debe incluir al menos un número.");

            if (!string.IsNullOrWhiteSpace(nombreUsuario) &&
                clave.IndexOf(nombreUsuario.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                errores.Add("No puede contener el nombre de usuario.");

            if (claveActual != null && clave == claveActual)
                errores.Add("Debe ser distinta de la contraseña actual.");

            return errores;
        }

        public static bool EsValida(string clave, string nombreUsuario, string? claveActual = null)
        {
            return Validar(clave, nombreUsuario, claveActual).Count == 0;
        }
    }
}