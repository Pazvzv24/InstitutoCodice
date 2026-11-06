using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a usuarios para login (RF-12).
    /// </summary>
    public class UsuarioRepositorio : RepositorioBase
    {
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            const string sql = @"
                SELECT u.Id, u.NombreUsuario, u.ContrasenaHash, u.Nombre, u.Rol, u.Activo, d.Rut, d.Correo
                FROM Usuario u
                LEFT JOIN Docente d ON d.Id = u.Id
                WHERE u.NombreUsuario = @NombreUsuario";

            return Ejecutar(conexion =>
            {
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    using (var lector = comando.ExecuteReader())
                    {
                        if (!lector.Read())
                            return null;

                        string rol = lector.GetString(lector.GetOrdinal("Rol"));
                        Usuario usuario;
                        if (rol == Docente.Rol)
                        {
                            usuario = new Docente
                            {
                                Rut = LeerTexto(lector, "Rut"),
                                Correo = LeerTexto(lector, "Correo")
                            };
                        }
                        else
                        {
                            usuario = new Administrador();
                        }

                        usuario.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                        usuario.NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario"));
                        usuario.ContrasenaHash = lector.GetString(lector.GetOrdinal("ContrasenaHash"));
                        usuario.Nombre = lector.GetString(lector.GetOrdinal("Nombre"));
                        usuario.Activo = lector.GetBoolean(lector.GetOrdinal("Activo"));
                        return usuario;
                    }
                }
            });
        }
    }
}
