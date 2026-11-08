using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de evaluaciones por asignatura, incluyendo su ponderación.
    /// Una evaluación solo se puede eliminar si no tiene notas: lo comprueba la consulta
    /// y, además, la clave foránea FK_Calificacion_Evaluacion (defensa en profundidad).
    /// Cada alta, modificación y eliminación queda en la bitácora, dentro de la misma transacción.
    /// </summary>
    public class EvaluacionRepositorio : RepositorioBase
    {
        private const string Entidad = "Evaluacion";

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

        public void Agregar(Evaluacion evaluacion, int asignaturaId, Usuario autor)
        {
            const string sql = @"
                INSERT INTO Evaluacion (AsignaturaId, Nombre, Fecha, Ponderacion)
                VALUES (@AsignaturaId, @Nombre, @Fecha, @Ponderacion);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    comando.Parameters.AddWithValue("@Nombre", evaluacion.Nombre);
                    comando.Parameters.AddWithValue("@Fecha", evaluacion.Fecha.Date);
                    comando.Parameters.AddWithValue("@Ponderacion", evaluacion.Ponderacion);
                    evaluacion.Id = (int)comando.ExecuteScalar();
                }

                string codigo = CodigoAsignatura(asignaturaId, conexion, transaccion);
                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.EvaluacionCreada,
                    "Evaluación \"" + evaluacion.Nombre + "\" (" + evaluacion.Ponderacion.ToString("0.##") + " %) en " + codigo + ".",
                    Entidad, evaluacion.Id, null, Describir(evaluacion.Nombre, evaluacion.Fecha, evaluacion.Ponderacion),
                    conexion, transaccion);
            });
        }

        public void Actualizar(Evaluacion evaluacion, Usuario autor)
        {
            const string sql = @"
                UPDATE Evaluacion
                SET Nombre = @Nombre, Fecha = @Fecha, Ponderacion = @Ponderacion
                WHERE Id = @Id";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // 1. Datos actuales, leídos de la base
                EvaluacionGuardada anterior = Leer(evaluacion.Id, conexion, transaccion)
                    ?? throw new InvalidOperationException("La evaluación ya no existe. Recargue la lista.");

                // 2. Guardar
                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Nombre", evaluacion.Nombre);
                    comando.Parameters.AddWithValue("@Fecha", evaluacion.Fecha.Date);
                    comando.Parameters.AddWithValue("@Ponderacion", evaluacion.Ponderacion);
                    comando.Parameters.AddWithValue("@Id", evaluacion.Id);
                    comando.ExecuteNonQuery();
                }

                // 3. Auditar
                AuditoriaRepositorio.Registrar(autor, AccionAuditoria.EvaluacionModificada,
                    "Evaluación (Id " + evaluacion.Id + ") de " + anterior.Codigo + " modificada.",
                    Entidad, evaluacion.Id,
                    Describir(anterior.Nombre, anterior.Fecha, anterior.Ponderacion),
                    Describir(evaluacion.Nombre, evaluacion.Fecha, evaluacion.Ponderacion),
                    conexion, transaccion);
            });
        }

        /// <summary>
        /// Elimina la evaluación solo si no tiene notas registradas.
        /// Devuelve false si no se eliminó porque tiene notas.
        /// </summary>
        public bool EliminarSiNoTieneNotas(int evaluacionId, Usuario autor)
        {
            const string sql = @"
                DELETE FROM Evaluacion
                WHERE Id = @Id
                  AND NOT EXISTS (SELECT 1 FROM Calificacion WHERE EvaluacionId = @Id)";

            bool eliminada = false;
            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                // Se lee ANTES de borrar, para que la bitácora sepa qué evaluación era
                EvaluacionGuardada? anterior = Leer(evaluacionId, conexion, transaccion);
                if (anterior == null)
                    return; // ya no existía

                using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                {
                    comando.Parameters.AddWithValue("@Id", evaluacionId);
                    eliminada = comando.ExecuteNonQuery() == 1;
                }

                if (eliminada)
                    AuditoriaRepositorio.Registrar(autor, AccionAuditoria.EvaluacionEliminada,
                        "Evaluación (Id " + evaluacionId + ") de " + anterior.Codigo + " eliminada (no tenía notas).",
                        Entidad, evaluacionId, Describir(anterior.Nombre, anterior.Fecha, anterior.Ponderacion), null,
                        conexion, transaccion);
            });
            return eliminada;
        }

        // ------------------------------------------------------------ Apoyo para la bitácora

        private sealed class EvaluacionGuardada
        {
            public string Nombre = string.Empty;
            public DateTime Fecha;
            public decimal Ponderacion;
            public string Codigo = string.Empty;
        }

        private static EvaluacionGuardada? Leer(int evaluacionId, SqlConnection conexion, SqlTransaction transaccion)
        {
            const string sql = @"
                SELECT e.Nombre, e.Fecha, e.Ponderacion, a.Codigo
                FROM Evaluacion e
                INNER JOIN Asignatura a ON a.Id = e.AsignaturaId
                WHERE e.Id = @Id";

            using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", evaluacionId);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                        return null;
                    return new EvaluacionGuardada
                    {
                        Nombre = lector.GetString(0),
                        Fecha = lector.GetDateTime(1),
                        Ponderacion = lector.GetDecimal(2),
                        Codigo = lector.GetString(3)
                    };
                }
            }
        }

        private static string CodigoAsignatura(int asignaturaId, SqlConnection conexion, SqlTransaction transaccion)
        {
            using (SqlCommand comando = new SqlCommand("SELECT Codigo FROM Asignatura WHERE Id = @Id", conexion, transaccion))
            {
                comando.Parameters.AddWithValue("@Id", asignaturaId);
                return comando.ExecuteScalar() as string ?? "Id " + asignaturaId;
            }
        }

        /// <summary>Ejemplo: "Prueba 1 | 04-09-2026 | 50 %".</summary>
        private static string Describir(string nombre, DateTime fecha, decimal ponderacion)
        {
            return nombre + " | " + fecha.ToString("dd-MM-yyyy") + " | " + ponderacion.ToString("0.##") + " %";
        }
    }
}