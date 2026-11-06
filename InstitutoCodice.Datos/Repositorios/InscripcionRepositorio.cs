using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de inscripciones de estudiantes en asignaturas.
    /// </summary>
    public class InscripcionRepositorio : RepositorioBase
    {
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

        public void Agregar(Inscripcion inscripcion)
        {
            const string sql = @"
                INSERT INTO Inscripcion (EstudianteId, AsignaturaId, FechaInscripcion)
                VALUES (@EstudianteId, @AsignaturaId, @FechaInscripcion);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@EstudianteId", inscripcion.Estudiante.Id);
                    comando.Parameters.AddWithValue("@AsignaturaId", inscripcion.Asignatura.Id);
                    comando.Parameters.AddWithValue("@FechaInscripcion", inscripcion.FechaInscripcion.Date);
                    inscripcion.Id = (int)comando.ExecuteScalar();
                }
            });
        }

        public void Eliminar(int inscripcionId)
        {
            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand("DELETE FROM Inscripcion WHERE Id = @Id", conexion))
                {
                    comando.Parameters.AddWithValue("@Id", inscripcionId);
                    comando.ExecuteNonQuery();
                }
            });
        }
    }
}