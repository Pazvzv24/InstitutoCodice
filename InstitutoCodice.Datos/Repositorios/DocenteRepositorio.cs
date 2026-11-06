using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de docentes, persistidos en las tablas Usuario y Docente.
    /// Un docente creado por el administrador queda obligado a cambiar su contraseña al primer ingreso.
    /// </summary>
    public class DocenteRepositorio : RepositorioBase
    {
        public List<Docente> Listar()
        {
            const string sql = @"
                SELECT u.Id, u.NombreUsuario, u.ContrasenaHash, u.Nombre, u.Activo, d.Rut, d.Correo
                FROM Docente d
                INNER JOIN Usuario u ON u.Id = d.Id
                ORDER BY u.Nombre";

            return Ejecutar(conexion =>
            {
                List<Docente> lista = new List<Docente>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Docente docente = new Docente();
                        docente.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                        docente.NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario"));
                        docente.ContrasenaHash = lector.GetString(lector.GetOrdinal("ContrasenaHash"));
                        docente.Nombre = lector.GetString(lector.GetOrdinal("Nombre"));
                        docente.Activo = lector.GetBoolean(lector.GetOrdinal("Activo"));
                        docente.Rut = lector.GetString(lector.GetOrdinal("Rut"));
                        docente.Correo = lector.GetString(lector.GetOrdinal("Correo"));
                        lista.Add(docente);
                    }
                }
                return lista;
            });
        }

        public void Agregar(Docente docente)
        {
            const string sqlUsuario = @"
                INSERT INTO Usuario (NombreUsuario, ContrasenaHash, Nombre, Rol, Activo, DebeCambiarContrasena)
                VALUES (@NombreUsuario, @ContrasenaHash, @Nombre, @Rol, @Activo, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            const string sqlDocente = "INSERT INTO Docente (Id, Rut, Correo) VALUES (@Id, @Rut, @Correo)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sqlUsuario, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@NombreUsuario", docente.NombreUsuario);
                    comando.Parameters.AddWithValue("@ContrasenaHash", docente.ContrasenaHash);
                    comando.Parameters.AddWithValue("@Nombre", docente.Nombre);
                    comando.Parameters.AddWithValue("@Rol", Docente.Rol);
                    comando.Parameters.AddWithValue("@Activo", docente.Activo);
                    docente.Id = (int)comando.ExecuteScalar();
                }

                using (SqlCommand comando = new SqlCommand(sqlDocente, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", docente.Id);
                    comando.Parameters.AddWithValue("@Rut", docente.Rut);
                    comando.Parameters.AddWithValue("@Correo", docente.Correo);
                    comando.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Actualiza los datos del docente y opcionalmente su contraseña.
        /// Si el administrador asigna una contraseña nueva, el docente deberá cambiarla al ingresar.
        /// </summary>
        public void Actualizar(Docente docente, bool cambiarContrasena)
        {
            string sqlUsuario = "UPDATE Usuario SET NombreUsuario = @NombreUsuario, Nombre = @Nombre, Activo = @Activo"
                              + (cambiarContrasena ? ", ContrasenaHash = @ContrasenaHash, DebeCambiarContrasena = 1" : "")
                              + " WHERE Id = @Id";
            const string sqlDocente = "UPDATE Docente SET Rut = @Rut, Correo = @Correo WHERE Id = @Id";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sqlUsuario, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@NombreUsuario", docente.NombreUsuario);
                    comando.Parameters.AddWithValue("@Nombre", docente.Nombre);
                    comando.Parameters.AddWithValue("@Activo", docente.Activo);
                    comando.Parameters.AddWithValue("@Id", docente.Id);
                    if (cambiarContrasena)
                        comando.Parameters.AddWithValue("@ContrasenaHash", docente.ContrasenaHash);
                    comando.ExecuteNonQuery();
                }

                using (SqlCommand comando = new SqlCommand(sqlDocente, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Rut", docente.Rut);
                    comando.Parameters.AddWithValue("@Correo", docente.Correo);
                    comando.Parameters.AddWithValue("@Id", docente.Id);
                    comando.ExecuteNonQuery();
                }
            });
        }
    }
}