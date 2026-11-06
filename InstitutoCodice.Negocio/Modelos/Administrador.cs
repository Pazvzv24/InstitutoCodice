namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Usuario con permisos de administración académica del sistema.
    /// </summary>
    public class Administrador : Usuario
    {
        public const string Rol = "Administrador";

        public override string ObtenerRol()
        {
            return Rol;
        }
    }
}