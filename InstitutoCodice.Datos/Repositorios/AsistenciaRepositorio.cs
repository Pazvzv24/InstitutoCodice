using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a datos de asistencia con guardado por curso y fecha.
    /// </summary>
    public class AsistenciaRepositorio : RepositorioBase
    {
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
        /// Guarda toda la lista en una transacción, insertando o actualizando según corresponda.
        /// </summary>
        public void GuardarLista(List<Asistencia> asistencias)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Asistencia
                           WHERE EstudianteId = @EstudianteId AND AsignaturaId = @AsignaturaId AND Fecha = @Fecha)
                    UPDATE Asistencia SET EstadoId = @EstadoId
                    WHERE EstudianteId = @EstudianteId AND AsignaturaId = @AsignaturaId AND Fecha = @Fecha
                ELSE
                    INSERT INTO Asistencia (EstudianteId, AsignaturaId, Fecha, EstadoId)
                    VALUES (@EstudianteId, @AsignaturaId, @Fecha, @EstadoId)";

            EjecutarEnTransaccion((conexion, transaccion) =>
            {
                foreach (Asistencia asistencia in asistencias)
                {
                    using (SqlCommand comando = new SqlCommand(sql, conexion, transaccion))
                    {
                        comando.Parameters.AddWithValue("@EstudianteId", asistencia.Estudiante.Id);
                        comando.Parameters.AddWithValue("@AsignaturaId", asistencia.Asignatura.Id);
                        comando.Parameters.AddWithValue("@Fecha", asistencia.Fecha.Date);
                        comando.Parameters.AddWithValue("@EstadoId", (byte)asistencia.Estado);
                        comando.ExecuteNonQuery();
                    }
                }
            });
        }
    }
}
