using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Define la base común para los usuarios autenticados del sistema.
    /// </summary>
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string ContrasenaHash { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }

        protected Usuario()
        {
            Activo = true;
        }

        /// <summary>
        /// Valida la clave proporcionada contra el hash almacenado.
        /// </summary>
        public bool VerificarContrasena(string clave)
        {
            return Seguridad.VerificarContrasena(clave, ContrasenaHash);
        }

        /// <summary>
        /// Obtiene el rol asociado al tipo de usuario.
        /// </summary>
        public abstract string ObtenerRol();

        public override string ToString()
        {
            return Nombre;
        }
    }
}