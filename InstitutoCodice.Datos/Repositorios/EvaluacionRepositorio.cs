using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de evaluaciones por asignatura, incluyendo su ponderación.
    /// Una evaluación solo se puede eliminar si no tiene notas: lo comprueba la consulta
    /// y, además, la clave foránea FK_Calificacion_Evaluacion (defensa en profundidad).
    /// </summary>
    public class EvaluacionRepositorio : RepositorioBase
    {
        public List<Evaluacion> ListarPorAsignatura(int asignaturaId)
        {
            const string sql = @"
                SELECT Id AS EvaluacionId, Nombre AS NombreEvaluacion, Fecha AS FechaEvaluacion, Ponderacion
                FROM Evaluacion
                WHERE AsignaturaId = @AsignaturaId
                ORDER BY Fecha, Id";

            return Ejecutar(conexion =>
            {
                List<Evaluacion> lista = new List<Evaluacion>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                            lista.Add(Mapeo.LeerEvaluacion(lector));
                    }
                }
                return lista;
            });
        }

        public void Agregar(Evaluacion evaluacion, int asignaturaId)
        {
            const string sql = @"
                INSERT INTO Evaluacion (AsignaturaId, Nombre, Fecha, Ponderacion)
                VALUES (@AsignaturaId, @Nombre, @Fecha, @Ponderacion);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    comando.Parameters.AddWithValue("@Nombre", evaluacion.Nombre);
                    comando.Parameters.AddWithValue("@Fecha", evaluacion.Fecha.Date);
                    comando.Parameters.AddWithValue("@Ponderacion", evaluacion.Ponderacion);
                    evaluacion.Id = (int)comando.ExecuteScalar();
                }
            });
        }

        public void Actualizar(Evaluacion evaluacion)
        {
            const string sql = @"
                UPDATE Evaluacion
                SET Nombre = @Nombre, Fecha = @Fecha, Ponderacion = @Ponderacion
                WHERE Id = @Id";

            Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", evaluacion.Nombre);
                    comando.Parameters.AddWithValue("@Fecha", evaluacion.Fecha.Date);
                    comando.Parameters.AddWithValue("@Ponderacion", evaluacion.Ponderacion);
                    comando.Parameters.AddWithValue("@Id", evaluacion.Id);
                    comando.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Elimina la evaluación solo si no tiene notas registradas.
        /// Devuelve false si no se eliminó porque tiene notas.
        /// </summary>
        public bool EliminarSiNoTieneNotas(int evaluacionId)
        {
            const string sql = @"
                DELETE FROM Evaluacion
                WHERE Id = @Id
                  AND NOT EXISTS (SELECT 1 FROM Calificacion WHERE EvaluacionId = @Id)";

            return Ejecutar(conexion =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Id", evaluacionId);
                    return comando.ExecuteNonQuery() == 1;
                }
            });
        }
    }
}