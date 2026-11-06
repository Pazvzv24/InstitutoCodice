using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Interfaces;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Proporciona a los servicios de informes los datos obtenidos desde la base de datos.
    /// </summary>
    public class InformeRepositorio : RepositorioBase, IFuenteDatosInformes
    {
        public List<Calificacion> ObtenerCalificaciones(int estudianteId, int asignaturaId)
        {
            const string sql = @"
                SELECT c.Id, c.Nota, ev.Id AS EvaluacionId, ev.Nombre AS NombreEvaluacion,
                       ev.Fecha AS FechaEvaluacion, ev.Ponderacion
                FROM Calificacion c
                INNER JOIN Evaluacion ev ON ev.Id = c.EvaluacionId
                WHERE c.EstudianteId = @EstudianteId AND ev.AsignaturaId = @AsignaturaId
                ORDER BY ev.Fecha, ev.Id";

            return Ejecutar(conexion =>
            {
                List<Calificacion> lista = new List<Calificacion>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Calificacion calificacion = new Calificacion();
                            calificacion.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                            calificacion.Nota = lector.GetDecimal(lector.GetOrdinal("Nota"));
                            calificacion.Evaluacion = Mapeo.LeerEvaluacion(lector);
                            lista.Add(calificacion);
                        }
                    }
                }
                return lista;
            });
        }

        public List<Asistencia> ObtenerAsistencias(int estudianteId, int asignaturaId)
        {
            const string sql = @"
                SELECT Id, Fecha, EstadoId
                FROM Asistencia
                WHERE EstudianteId = @EstudianteId AND AsignaturaId = @AsignaturaId";

            return Ejecutar(conexion =>
            {
                List<Asistencia> lista = new List<Asistencia>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Asistencia asistencia = new Asistencia();
                            asistencia.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                            asistencia.Fecha = lector.GetDateTime(lector.GetOrdinal("Fecha"));
                            asistencia.Estado = (EstadoAsistencia)lector.GetByte(lector.GetOrdinal("EstadoId"));
                            lista.Add(asistencia);
                        }
                    }
                }
                return lista;
            });
        }

        public List<Asignatura> ObtenerAsignaturasDeEstudiante(int estudianteId)
        {
            const string sql = @"
                SELECT a.Id, a.Codigo, a.Nombre, a.Curso, a.DocenteId, u.Nombre AS NombreDocente
                FROM Inscripcion i
                INNER JOIN Asignatura a ON a.Id = i.AsignaturaId
                INNER JOIN Usuario u ON u.Id = a.DocenteId
                WHERE i.EstudianteId = @EstudianteId
                ORDER BY a.Nombre";

            return Ejecutar(conexion =>
            {
                List<Asignatura> lista = new List<Asignatura>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@EstudianteId", estudianteId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                            lista.Add(Mapeo.LeerAsignatura(lector));
                    }
                }
                return lista;
            });
        }

        public List<Estudiante> ObtenerInscritos(int asignaturaId)
        {
            const string sql = @"
                SELECT e.Id, e.Rut, e.Nombres, e.Apellidos, e.Correo, e.Curso, e.Activo, e.TratamientoBloqueado
                FROM Inscripcion i
                INNER JOIN Estudiante e ON e.Id = i.EstudianteId
                WHERE i.AsignaturaId = @AsignaturaId
                ORDER BY e.Apellidos, e.Nombres";

            return Ejecutar(conexion =>
            {
                List<Estudiante> lista = new List<Estudiante>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                            lista.Add(Mapeo.LeerEstudiante(lector));
                    }
                }
                return lista;
            });
        }
    }
}