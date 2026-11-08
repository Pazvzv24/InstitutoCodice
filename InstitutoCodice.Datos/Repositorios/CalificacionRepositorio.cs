using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de calificaciones por evaluación con guardado en lote.
    /// Cada nota registrada, modificada o eliminada queda en la bitácora de auditoría,
    /// dentro de la misma transacción: si algo falla, no se guarda nada.
    /// </summary>
    public class CalificacionRepositorio : RepositorioBase
    {
        public List<Calificacion> ListarPorEvaluacion(int evaluacionId)
        {
            const string sql = "SELECT Id, EstudianteId, Nota FROM Calificacion WHERE EvaluacionId = @EvaluacionId";

            return Ejecutar(conexion =>
            {
                List<Calificacion> lista = new List<Calificacion>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@EvaluacionId", evaluacionId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Calificacion calificacion = new Calificacion();
                            calificacion.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                            calificacion.Estudiante = new Estudiante();
                            calificacion.Estudiante.Id = lector.GetInt32(lector.GetOrdinal("EstudianteId"));
                            calificacion.Nota = lector.GetDecimal(lector.GetOrdinal("Nota"));
                            lista.Add(calificacion);
                        }
                    }
                }
                return lista;
            });
        }

        /// <summary>
        /// Guarda todas las notas de la evaluación, actualizando si ya existen,
        /// y registra en la auditoría quién las ingresó o modificó.
        /// </summary>
        public void GuardarLista(List<Calificacion> calificaciones, Usuario autor)
        {
            GuardarCambios(calificaciones, null, new List<int>(), autor);
        }

        /// <summary>
        /// Guarda las notas nuevas o modificadas y elimina las notas de los estudiantes
        /// indicados (celdas que quedaron vacías), todo en una sola transacción.
        /// Cada eliminación queda en la bitácora con la nota que tenía.
        /// </summary>
        public void GuardarCambios(List<Calificacion> calificaciones, Evaluacion? evaluacion,
                                   IReadOnlyCollection<int> estudiantesSinNota, Usuario autor)
        {
            // Trae el RUT del estudiante y, si ya existe, su nota en esta evaluación
            const string sqlBuscar = @"
                SELECT e.Rut, c.Id, c.Nota
                FROM Estudiante e
                LEFT JOIN Calificacion c
                       ON c.EstudianteId = e.Id AND c.EvaluacionId = @EvaluacionId
                WHERE e.Id = @EstudianteId";
            const string sqlActualizar = "UPDATE Calificacion SET Nota = @Nota WHERE Id = @Id";
            const string sqlInsertar = @"
                INSERT INTO Calificacion (EstudianteId, EvaluacionId, Nota)
                OUTPUT INSERTED.Id
                VALUES (@EstudianteId, @EvaluacionId, @Nota)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                foreach (Calificacion calificacion in calificaciones)
                {
                    string? rut = null;
                    int? idExistente = null;
                    decimal? notaAnterior = null;

                    using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion, transaccion))
                    {
                        comando.Parameters.AddWithValue("@EstudianteId", calificacion.Estudiante.Id);
                        comando.Parameters.AddWithValue("@EvaluacionId", calificacion.Evaluacion.Id);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                rut = lector.GetString(0);
                                if (!lector.IsDBNull(1))
                                {
                                    idExistente = lector.GetInt32(1);
                                    notaAnterior = lector.GetDecimal(2);
                                }
                            }
                        }
                    }

                    string? accion = AccionAuditoria.AccionParaNota(notaAnterior, calificacion.Nota);
                    if (accion == null)
                        continue; // Misma nota: no hay cambio que guardar ni auditar

                    int idCalificacion;
                    if (idExistente.HasValue)
                    {
                        using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@Nota", calificacion.Nota);
                            comando.Parameters.AddWithValue("@Id", idExistente.Value);
                            comando.ExecuteNonQuery();
                        }
                        idCalificacion = idExistente.Value;
                    }
                    else
                    {
                        using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@EstudianteId", calificacion.Estudiante.Id);
                            comando.Parameters.AddWithValue("@EvaluacionId", calificacion.Evaluacion.Id);
                            comando.Parameters.AddWithValue("@Nota", calificacion.Nota);
                            idCalificacion = (int)comando.ExecuteScalar();
                        }
                    }

                    AuditoriaRepositorio.Registrar(new RegistroAuditoria
                    {
                        UsuarioId = autor.Id,
                        NombreUsuario = autor.NombreUsuario,
                        Accion = accion,
                        Detalle = AccionAuditoria.DetalleNota(calificacion.Evaluacion.Nombre, calificacion.Evaluacion.Id,
                                                              rut, calificacion.Estudiante.Id),
                        Entidad = "Calificacion",
                        EntidadId = idCalificacion,
                        ValorAnterior = notaAnterior?.ToString("0.0", CultureInfo.InvariantCulture),
                        ValorNuevo = calificacion.Nota.ToString("0.0", CultureInfo.InvariantCulture)
                    }, conexion, transaccion);
                }

                if (evaluacion != null)
                {
                    foreach (int estudianteId in estudiantesSinNota)
                        EliminarNota(evaluacion, estudianteId, autor, conexion, transaccion);
                }
            });
        }

        private static void EliminarNota(Evaluacion evaluacion, int estudianteId, Usuario autor,
                                         SqlConnection conexion, SqlTransaction transaccion)
        {
            const string sqlBuscar = @"
                SELECT c.Id, c.Nota, e.Rut
                FROM Calificacion c
                INNER JOIN Estudiante e ON e.Id = c.EstudianteId
                WHERE c.EstudianteId = @EstudianteId AND c.EvaluacionId = @EvaluacionId";
            const string sqlEliminar = "DELETE FROM Calificacion WHERE Id = @Id";

            int id;
            decimal notaAnterior;
            string rut;
            using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@EstudianteId", estudianteId);
                comando.Parameters.AddWithValue("@EvaluacionId", evaluacion.Id);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                        return; // no tenía nota: no hay nada que eliminar
                    id = lector.GetInt32(0);
                    notaAnterior = lector.GetDecimal(1);
                    rut = lector.GetString(2);
                }
            }

            using (SqlCommand comando = new SqlCommand(sqlEliminar, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }

            AuditoriaRepositorio.Registrar(new RegistroAuditoria
            {
                UsuarioId = autor.Id,
                NombreUsuario = autor.NombreUsuario,
                Accion = AccionAuditoria.NotaEliminada,
                Detalle = AccionAuditoria.DetalleNota(evaluacion.Nombre, evaluacion.Id, rut, estudianteId),
                Entidad = "Calificacion",
                EntidadId = id,
                ValorAnterior = notaAnterior.ToString("0.0", CultureInfo.InvariantCulture)
            }, conexion, transaccion);
        }
    }
}