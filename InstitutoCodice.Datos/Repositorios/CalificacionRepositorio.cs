using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de calificaciones por evaluación con guardado en lote.
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
        /// Guarda todas las notas de la evaluación, actualizando si ya existen.
        /// </summary>
        public void GuardarLista(List<Calificacion> calificaciones)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Calificacion WHERE EstudianteId = @EstudianteId AND EvaluacionId = @EvaluacionId)
                    UPDATE Calificacion SET Nota = @Nota
                    WHERE EstudianteId = @EstudianteId AND EvaluacionId = @EvaluacionId
                ELSE
                    INSERT INTO Calificacion (EstudianteId, EvaluacionId, Nota)
                    VALUES (@EstudianteId, @EvaluacionId, @Nota)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                foreach (Calificacion calificacion in calificaciones)
                {
                    using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                    {
                        comando.Parameters.AddWithValue("@EstudianteId", calificacion.Estudiante.Id);
                        comando.Parameters.AddWithValue("@EvaluacionId", calificacion.Evaluacion.Id);
                        comando.Parameters.AddWithValue("@Nota", calificacion.Nota);
                        comando.ExecuteNonQuery();
                    }
                }
            });
        }
    }
}