using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de docentes, persistidos en las tablas Usuario y Docente.
    /// Un docente creado por el administrador queda obligado a cambiar su contraseña al primer ingreso.
    /// Cada alta, modificación, asignación de contraseña, desactivación y reactivación queda en la
    /// bitácora dentro de la misma transacción. Minimización: del nombre, RUT y correo del docente
    /// se registra solo que cambiaron, no su valor.
    /// </summary>
    public class DocenteRepositorio : RepositorioBase
    {
        private const string Entidad = "Usuario";

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

        public void Agregar(Docente docente, Usuario autor)
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

                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.UsuarioCreado,
                    "Docente " + docente.NombreUsuario + " creado.",
                    Entidad, docente.Id, null, null, conexion, transaccion);
            });
        }

        /// <summary>
        /// Actualiza los datos del docente y opcionalmente su contraseña.
        /// Si el administrador asigna una contraseña nueva, el docente deberá cambiarla al ingresar.
        /// </summary>
        public void Actualizar(Docente docente, bool cambiarContrasena, Usuario autor)
        {
            const string sqlAnterior = @"
                SELECT u.NombreUsuario, u.Nombre, u.Activo, d.Rut, d.Correo
                FROM Usuario u
                INNER JOIN Docente d ON d.Id = u.Id
                WHERE u.Id = @Id";
            string sqlUsuario = "UPDATE Usuario SET NombreUsuario = @NombreUsuario, Nombre = @Nombre, Activo = @Activo"
                              + (cambiarContrasena ? ", ContrasenaHash = @ContrasenaHash, DebeCambiarContrasena = 1" : "")
                              + " WHERE Id = @Id";
            const string sqlDocente = "UPDATE Docente SET Rut = @Rut, Correo = @Correo WHERE Id = @Id";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // 1. Datos actuales, leídos de la base
                string usuario, nombre, rut, correo;
                bool activo;
                using (SqlCommand comando = new SqlCommand(sqlAnterior, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", docente.Id);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (!lector.Read())
                            throw new InvalidOperationException("El docente ya no existe. Recargue la lista.");

                        usuario = lector.GetString(0);
                        nombre = lector.GetString(1);
                        activo = lector.GetBoolean(2);
                        rut = lector.GetString(3);
                        correo = lector.GetString(4);
                    }
                }

                // 2. Guardar
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

                // 3. Auditar cada tipo de cambio
                ResumenCambios cambios = CambiosAuditoria.Resumir(
                    new CampoAuditado("Usuario", usuario, docente.NombreUsuario),
                    new CampoAuditado("Nombre", nombre, docente.Nombre, esDatoPersonal: true),
                    new CampoAuditado("RUT", rut, docente.Rut, esDatoPersonal: true),
                    new CampoAuditado("Correo", correo, docente.Correo, esDatoPersonal: true));

                if (cambios.HayCambios)
                    AuditoriaRepositorio.Registrar(autor, AccionAuditoria.DocenteModificado,
                        "Docente " + docente.NombreUsuario + ": se modificó " + cambios.TextoCampos + ".",
                        Entidad, docente.Id, cambios.ValorAnterior, cambios.ValorNuevo, conexion, transaccion);

                if (cambiarContrasena)
                    AuditoriaRepositorio.Registrar(autor, AccionAuditoria.ContrasenaAsignada,
                        "Contraseña asignada por el administrador a " + docente.NombreUsuario + ".",
                        Entidad, docente.Id, null, null, conexion, transaccion);

                if (activo != docente.Activo)
                    AuditoriaRepositorio.Registrar(autor,
                        docente.Activo ? AccionAuditoria.UsuarioReactivado : AccionAuditoria.UsuarioDesactivado,
                        "Docente " + docente.NombreUsuario + (docente.Activo ? " reactivado." : " desactivado."),
                        Entidad, docente.Id,
                        activo ? "Activo" : "Inactivo", docente.Activo ? "Activo" : "Inactivo",
                        conexion, transaccion);
            });
        }
    }
}