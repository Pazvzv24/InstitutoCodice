using System;
using System.Security.Cryptography;
using System.Text;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Almacenamiento y verificación segura de contraseñas (RNF-03).
    ///
    /// Algoritmo: PBKDF2 con HMAC-SHA256, sal aleatoria de 16 bytes y
    /// 600.000 iteraciones (recomendación OWASP vigente para PBKDF2-SHA256).
    /// Al ser un algoritmo lento a propósito, encarece los ataques de fuerza bruta.
    ///
    /// Formato versionado del hash guardado:
    ///     PBKDF2$iteraciones$sal_base64$hash_base64
    /// El prefijo y las iteraciones quedan dentro del valor, así el sistema
    /// puede subir las iteraciones o cambiar de algoritmo en el futuro
    /// sin invalidar los hashes existentes.
    /// </summary>
    public static class Seguridad
    {
        private const string Algoritmo = "PBKDF2";
        private const int Iteraciones = 600_000;
        private const int LargoSal = 16;   // 128 bits
        private const int LargoHash = 32;  // 256 bits
        private static readonly HashAlgorithmName Prf = HashAlgorithmName.SHA256;

        /// <summary>
        /// Genera el hash de una contraseña con una sal aleatoria nueva.
        /// Dos llamadas con la misma clave producen valores distintos.
        /// </summary>
        public static string GenerarHash(string clave)
        {
            if (string.IsNullOrEmpty(clave))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(clave));

            byte[] sal = RandomNumberGenerator.GetBytes(LargoSal);
            byte[] hash = Derivar(clave, sal, Iteraciones);

            return string.Join("$",
                Algoritmo,
                Iteraciones.ToString(),
                Convert.ToBase64String(sal),
                Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Verifica una contraseña contra el hash guardado.
        /// La comparación es sensible a mayúsculas: "Admin123" != "admin123".
        /// Ante cualquier formato inválido devuelve false (nunca lanza excepción al login).
        /// </summary>
        public static bool VerificarContrasena(string clave, string hashGuardado)
        {
            if (string.IsNullOrEmpty(clave) || string.IsNullOrEmpty(hashGuardado))
                return false;

            string[] partes = hashGuardado.Split('$');
            if (partes.Length != 4 || partes[0] != Algoritmo)
                return false;

            if (!int.TryParse(partes[1], out int iteraciones) || iteraciones <= 0)
                return false;

            try
            {
                byte[] sal = Convert.FromBase64String(partes[2]);
                byte[] esperado = Convert.FromBase64String(partes[3]);
                byte[] calculado = Derivar(clave, sal, iteraciones, esperado.Length);

                // Comparación en tiempo constante: evita ataques de temporización
                return CryptographicOperations.FixedTimeEquals(esperado, calculado);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si un hash fue generado con parámetros más débiles que los actuales
        /// (por ejemplo, menos iteraciones). Útil para re-generarlo en el próximo login.
        /// </summary>
        public static bool RequiereActualizacion(string hashGuardado)
        {
            if (string.IsNullOrEmpty(hashGuardado))
                return true;

            string[] partes = hashGuardado.Split('$');
            return partes.Length != 4
                || partes[0] != Algoritmo
                || !int.TryParse(partes[1], out int iteraciones)
                || iteraciones < Iteraciones;
        }

        private static byte[] Derivar(string clave, byte[] sal, int iteraciones, int largo = LargoHash)
        {
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(clave), sal, iteraciones, Prf, largo);
        }
    }
}