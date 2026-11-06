using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de calificaciones por evaluación con guardado en lote.
    /// Cada nota registrada o modificada queda en la bitácora de auditoría,
    /// dentro de la misma transacción.
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
            const string sqlBuscar = @"
                SELECT Id, Nota FROM Calificacion
                WHERE EstudianteId = @EstudianteId AND EvaluacionId = @EvaluacionId";
            const string sqlActualizar = "UPDATE Calificacion SET Nota = @Nota WHERE Id = @Id";
            const string sqlInsertar = @"
                INSERT INTO Calificacion (EstudianteId, EvaluacionId, Nota)
                OUTPUT INSERTED.Id
                VALUES (@EstudianteId, @EvaluacionId, @Nota)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                foreach (Calificacion calificacion in calificaciones)
                {
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
                                idExistente = lector.GetInt32(0);
                                notaAnterior = lector.GetDecimal(1);
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
                        Detalle = "Evaluación \"" + calificacion.Evaluacion.Nombre + "\" (Id " + calificacion.Evaluacion.Id
                                + "), estudiante Id " + calificacion.Estudiante.Id,
                        Entidad = "Calificacion",
                        EntidadId = idCalificacion,
                        ValorAnterior = notaAnterior?.ToString("0.0", CultureInfo.InvariantCulture),
                        ValorNuevo = calificacion.Nota.ToString("0.0", CultureInfo.InvariantCulture)
                    }, conexion, transaccion);
                }
            });
        }
    }
}