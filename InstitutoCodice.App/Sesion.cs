using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App
{
    /// <summary>
    /// Mantiene el usuario autenticado en memoria durante la sesión de la app.
    /// </summary>
    public static class Sesion
    {
        public static Usuario UsuarioActual { get; private set; }

        /// <summary>
        /// Marca si se salió por cierre de sesión voluntario para volver al login.
        /// </summary>
        public static bool CerroSesion { get; set; }

        public static bool EsAdministrador
        {
            get { return UsuarioActual is Administrador; }
        }

        public static void Iniciar(Usuario usuario)
        {
            UsuarioActual = usuario;
            CerroSesion = false;
        }

        public static void Cerrar()
        {
            UsuarioActual = null;
        }
    }
}
