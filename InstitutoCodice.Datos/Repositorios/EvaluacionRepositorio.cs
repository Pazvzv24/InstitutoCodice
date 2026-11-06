using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de evaluaciones por asignatura, incluyendo su ponderación.
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
    }
}
