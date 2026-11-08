using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de asistencia con guardado por curso y fecha.
    /// Cada asistencia registrada o modificada queda en la bitácora de auditoría,
    /// dentro de la misma transacción: si algo falla, no se guarda nada.
    /// </summary>
    public class AsistenciaRepositorio : RepositorioBase
    {
        private const string Entidad = "Asistencia";

        public List<Asistencia> ListarPorAsignaturaYFecha(int asignaturaId, DateTime fecha)
        {
            const string sql = @"
                SELECT Id, EstudianteId, Fecha, EstadoId
                FROM Asistencia
                WHERE AsignaturaId = @AsignaturaId AND Fecha = @Fecha";

            return Ejecutar(conexion =>
            {
                List<Asistencia> lista = new List<Asistencia>();
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@AsignaturaId", asignaturaId);
                    comando.Parameters.AddWithValue("@Fecha", fecha.Date);
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Asistencia asistencia = new Asistencia();
                            asistencia.Id = lector.GetInt32(lector.GetOrdinal("Id"));
                            asistencia.Estudiante = new Estudiante();
                            asistencia.Estudiante.Id = lector.GetInt32(lector.GetOrdinal("EstudianteId"));
                            asistencia.Fecha = lector.GetDateTime(lector.GetOrdinal("Fecha"));
                            asistencia.Estado = (EstadoAsistencia)lector.GetByte(lector.GetOrdinal("EstadoId"));
                            lista.Add(asistencia);
                        }
                    }
                }
                return lista;
            });
        }

        /// <summary>
        /// Guarda toda la lista en una transacción, insertando o actualizando según corresponda,
        /// y registra en la bitácora quién registró o modificó cada asistencia.
        /// Si el estado no cambió, no se guarda ni se audita nada para ese estudiante.
        /// </summary>
        public void GuardarLista(List<Asistencia> asistencias, Usuario autor)
        {
            // Trae el RUT del estudiante y, si ya existe, su asistencia en esa asignatura y fecha
            const string sqlBuscar = @"
                SELECT e.Rut, a.Id, a.EstadoId
                FROM Estudiante e
                LEFT JOIN Asistencia a
                       ON a.EstudianteId = e.Id AND a.AsignaturaId = @AsignaturaId AND a.Fecha = @Fecha
                WHERE e.Id = @EstudianteId";
            const string sqlActualizar = "UPDATE Asistencia SET EstadoId = @EstadoId WHERE Id = @Id";
            const string sqlInsertar = @"
                INSERT INTO Asistencia (EstudianteId, AsignaturaId, Fecha, EstadoId)
                OUTPUT INSERTED.Id
                VALUES (@EstudianteId, @AsignaturaId, @Fecha, @EstadoId)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                foreach (Asistencia asistencia in asistencias)
                {
                    string? rut = null;
                    int? idExistente = null;
                    EstadoAsistencia? estadoAnterior = null;

                    using (SqlCommand comando = new SqlCommand(sqlBuscar, conexion, transaccion))
                    {
                        comando.Parameters.AddWithValue("@EstudianteId", asistencia.Estudiante.Id);
                        comando.Parameters.AddWithValue("@AsignaturaId", asistencia.Asignatura.Id);
                        comando.Parameters.AddWithValue("@Fecha", asistencia.Fecha.Date);
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                rut = lector.GetString(0);
                                if (!lector.IsDBNull(1))
                                {
                                    idExistente = lector.GetInt32(1);
                                    estadoAnterior = (EstadoAsistencia)lector.GetByte(2);
                                }
                            }
                        }
                    }

                    string? accion = AccionAuditoria.AccionParaAsistencia(estadoAnterior, asistencia.Estado);
                    if (accion == null)
                        continue; // Mismo estado: no hay cambio que guardar ni auditar

                    int idAsistencia;
                    if (idExistente.HasValue)
                    {
                        using (SqlCommand comando = new SqlCommand(sqlActualizar, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@EstadoId", (byte)asistencia.Estado);
                            comando.Parameters.AddWithValue("@Id", idExistente.Value);
                            comando.ExecuteNonQuery();
                        }
                        idAsistencia = idExistente.Value;
                    }
                    else
                    {
                        using (SqlCommand comando = new SqlCommand(sqlInsertar, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@EstudianteId", asistencia.Estudiante.Id);
                            comando.Parameters.AddWithValue("@AsignaturaId", asistencia.Asignatura.Id);
                            comando.Parameters.AddWithValue("@Fecha", asistencia.Fecha.Date);
                            comando.Parameters.AddWithValue("@EstadoId", (byte)asistencia.Estado);
                            idAsistencia = (int)comando.ExecuteScalar();
                        }
                    }

                    AuditoriaRepositorio.Registrar(autor, accion,
                        AccionAuditoria.DetalleAsistencia(asistencia.Asignatura.Codigo, asistencia.Fecha,
                                                          rut, asistencia.Estudiante.Id),
                        Entidad, idAsistencia,
                        estadoAnterior?.ToString(), asistencia.Estado.ToString(),
                        conexion, transaccion);
                }
            });
        }
    }
}