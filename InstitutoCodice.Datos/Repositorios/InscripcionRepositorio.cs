using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de inscripciones de estudiantes en asignaturas.
    /// Cada inscripción y cada anulación queda en la bitácora, dentro de la misma transacción.
    /// </summary>
    public class InscripcionRepositorio : RepositorioBase
    {
        private const string Entidad = "Inscripcion";

        public List<Inscripcion> ListarPorAsignatura(int asignaturaId)
        {
            const string sql = @"
                SELECT i.Id AS InscripcionId, i.FechaInscripcion,
                       e.Id, e.Rut, e.Nombres, e.Apellidos, e.Correo, e.Curso, e.Activo
                FROM Inscripcion i
                INNER JOIN Estudiante e ON e.Id = i.EstudianteId
                WHERE i.AsignaturaId = @AsignaturaId
                ORDER BY e.Apellidos, e.Nombres";

            return Ejecutar(conexion =>
            {
                List<Inscripcion> lista = new List<Inscripcion>();
                Asignatura asignatura = new Asignatura();
                asignatura.Id = asignaturaId;

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Inscripcion inscripcion = new Inscripcion();
                            inscripcion.Id = lector.GetInt32(lector.GetOrdinal("InscripcionId"));
                            inscripcion.FechaInscripcion = lector.GetDateTime(lector.GetOrdinal("FechaInscripcion"));
                            inscripcion.Estudiante = Mapeo.LeerEstudiante(lector);
                            inscripcion.Asignatura = asignatura;
                            lista.Add(inscripcion);
                        }
                    }
                }
                return lista;
            });
        }

        public void Agregar(Inscripcion inscripcion, Usuario autor)
        {
            const string sql = @"
                INSERT INTO Inscripcion (EstudianteId, AsignaturaId, FechaInscripcion)
                VALUES (@EstudianteId, @AsignaturaId, @FechaInscripcion);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@EstudianteId", inscripcion.Estudiante.Id);
                    comando.Parameters.AddWithValue("@AsignaturaId", inscripcion.Asignatura.Id);
                    comando.Parameters.AddWithValue("@FechaInscripcion", inscripcion.FechaInscripcion.Date);
                    inscripcion.Id = (int)comando.ExecuteScalar();
                }

                string? descripcion = Describir(inscripcion.Id, conexion, transaccion);
                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.InscripcionCreada,
                    "Inscripción creada: " + descripcion + ".",
                    Entidad, inscripcion.Id, null, null, conexion, transaccion);
            });
        }

        public void Eliminar(int inscripcionId, Usuario autor)
        {
            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // Se describe ANTES de borrar, para que la bitácora sepa qué inscripción era
                string? descripcion = Describir(inscripcionId, conexion, transaccion);
                if (descripcion == null)
                    return; // ya no existía: no hay nada que borrar ni auditar

                using (SqlCommand comando = new SqlCommand("DELETE FROM Inscripcion WHERE Id = @Id", conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", inscripcionId);
                    comando.ExecuteNonQuery();
                }

                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.InscripcionEliminada,
                    "Inscripción anulada: " + descripcion + ".",
                    Entidad, inscripcionId, null, null, conexion, transaccion);
            });
        }

        /// <summary>
        /// Ejemplo: "estudiante RUT 20456789-1 en BDD201 (TI-301)". Null si la inscripción no existe.
        /// Identifica al estudiante solo por RUT (minimización de datos).
        /// </summary>
        private static string? Describir(int inscripcionId, SqlConnection conexion, SqlTransaction transaccion)
        {
            const string sql = @"
                SELECT e.Rut, a.Codigo, a.Curso
                FROM Inscripcion i
                INNER JOIN Estudiante e ON e.Id = i.EstudianteId
                INNER JOIN Asignatura a ON a.Id = i.AsignaturaId
                WHERE i.Id = @Id";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", inscripcionId);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                        return null;
                    return "estudiante RUT " + lector.GetString(0) + " en " + lector.GetString(1) + " (" + lector.GetString(2) + ")";
                }
            }
        }
    }
}