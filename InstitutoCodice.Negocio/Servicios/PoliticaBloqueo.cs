using System;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Reglas de bloqueo de cuenta por intentos fallidos (RF-12 / RNF-03).
    ///
    /// - Tras 3 intentos fallidos seguidos, la cuenta se bloquea 15 minutos.
    /// - El bloqueo se guarda en la base de datos: reiniciar la app no lo levanta.
    /// - Mientras la cuenta está bloqueada no se verifica la contraseña,
    ///   así un atacante no puede seguir probando claves.
    /// - Un ingreso correcto reinicia el contador.
    ///
    /// Las fechas se manejan en UTC. El momento actual ("ahora") se recibe como
    /// parámetro para poder probar las reglas sin depender del reloj real.
    /// </summary>
    public static class PoliticaBloqueo
    {
        public const int MaxIntentos = 3;
        public static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Indica si la cuenta está bloqueada en este momento.
        /// </summary>
        public static bool EstaBloqueado(Usuario usuario, DateTime ahoraUtc)
        {
            return usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > ahoraUtc;
        }

        /// <summary>
        /// Minutos que faltan para que se levante el bloqueo (redondeado hacia arriba).
        /// </summary>
        public static int MinutosRestantes(Usuario usuario, DateTime ahoraUtc)
        {
            if (!EstaBloqueado(usuario, ahoraUtc))
                return 0;

            return (int)Math.Ceiling((usuario.BloqueadoHasta!.Value - ahoraUtc).TotalMinutes);
        }

        /// <summary>
        /// Registra un intento fallido. Devuelve true si con este intento la cuenta quedó bloqueada.
        /// </summary>
        public static bool RegistrarFallo(Usuario usuario, DateTime ahoraUtc)
        {
            // Si había un bloqueo que ya expiró, se parte de cero
            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value <= ahoraUtc)
            {
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta = null;
            }

            usuario.IntentosFallidos++;

            if (usuario.IntentosFallidos >= MaxIntentos)
            {
                usuario.BloqueadoHasta = ahoraUtc.Add(DuracionBloqueo);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ingreso correcto: reinicia el contador y quita cualquier bloqueo.
        /// </summary>
        public static void RegistrarExito(Usuario usuario)
        {
            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;
        }
    }
}