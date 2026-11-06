using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a usuarios para login (RF-12) y control de bloqueo de cuenta.
    /// </summary>
    public class UsuarioRepositorio : RepositorioBase
    {
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            const string sql = @"
                SELECT u.Id, u.NombreUsuario, u.ContrasenaHash, u.Nombre, u.Rol, u.Activo,
                       u.IntentosFallidos, u.BloqueadoHasta, d.Rut, d.Correo
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
                        usuario.IntentosFallidos = lector.GetInt32(lector.GetOrdinal("IntentosFallidos"));

                        int ordinalBloqueo = lector.GetOrdinal("BloqueadoHasta");
                        usuario.BloqueadoHasta = lector.IsDBNull(ordinalBloqueo)
                            ? (DateTime?)null
                            : DateTime.SpecifyKind(lector.GetDateTime(ordinalBloqueo), DateTimeKind.Utc);

                        return usuario;
                    }
                }
            });
        }

        /// <summary>
        /// Guarda el contador de intentos fallidos y el bloqueo de la cuenta.
        /// </summary>
        public void ActualizarEstadoAcceso(Usuario usuario)
        {
            const string sql = @"
                UPDATE Usuario
                SET IntentosFallidos = @IntentosFallidos,
                    BloqueadoHasta = @BloqueadoHasta
                WHERE Id = @Id";

            Ejecutar(conexion =>
            {
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@IntentosFallidos", usuario.IntentosFallidos);
                    comando.Parameters.Add("@BloqueadoHasta", System.Data.SqlDbType.DateTime2).Value =
                        usuario.BloqueadoHasta.HasValue ? (object)usuario.BloqueadoHasta.Value : DBNull.Value;
                    comando.Parameters.AddWithValue("@Id", usuario.Id);
                    return comando.ExecuteNonQuery();
                }
            });
        }
    }
}