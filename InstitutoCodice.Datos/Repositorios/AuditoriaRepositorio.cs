using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Acceso a la bitácora de auditoría. Solo inserta y consulta:
    /// la base de datos rechaza modificaciones y eliminaciones.
    /// </summary>
    public class AuditoriaRepositorio : RepositorioBase
    {
        private const string SqlInsertar = @"
            INSERT INTO Auditoria (UsuarioId, NombreUsuario, Accion, Detalle, Entidad, EntidadId, ValorAnterior, ValorNuevo)
            VALUES (@UsuarioId, @NombreUsuario, @Accion, @Detalle, @Entidad, @EntidadId, @ValorAnterior, @ValorNuevo)";

        public void Registrar(RegistroAuditoria registro)
        {
            Ejecutar(conexion =>
            {
                using (var comando = new SqlCommand(SqlInsertar, conexion))
                {
                    AgregarParametros(comando, registro);
                    return comando.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Registra dentro de una transacción existente (por ejemplo, junto con el guardado de notas),
        /// para que el cambio y su auditoría se guarden juntos o no se guarde ninguno.
        /// </summary>
        public static void Registrar(RegistroAuditoria registro, SqlConnection conexion, SqlTransaction transaccion)
        {
            using (var comando = new SqlCommand(SqlInsertar, conexion, transaccion))
            {
                AgregarParametros(comando, registro);
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Consulta los registros entre dos fechas (UTC), con filtros opcionales. Máximo 500, los más recientes primero.
        /// </summary>
        public List<RegistroAuditoria> Listar(DateTime desdeUtc, DateTime hastaUtc, string? accion, string? nombreUsuario)
        {
            const string sql = @"
                SELECT TOP (500) Id, FechaUtc, UsuarioId, NombreUsuario, Accion, Detalle,
                       Entidad, EntidadId, ValorAnterior, ValorNuevo
                FROM Auditoria
                WHERE FechaUtc >= @Desde AND FechaUtc < @Hasta
                  AND (@Accion IS NULL OR Accion = @Accion)
                  AND (@Usuario IS NULL OR NombreUsuario LIKE '%' + @Usuario + '%')
                ORDER BY FechaUtc DESC, Id DESC";

            return Ejecutar(conexion =>
            {
                var lista = new List<RegistroAuditoria>();
                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = desdeUtc;
                    comando.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = hastaUtc;
                    comando.Parameters.Add("@Accion", SqlDbType.NVarChar, 40).Value =
                        string.IsNullOrWhiteSpace(accion) ? DBNull.Value : accion;
                    comando.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrWhiteSpace(nombreUsuario) ? DBNull.Value : nombreUsuario.Trim();

                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new RegistroAuditoria
                            {
                                Id = lector.GetInt64(lector.GetOrdinal("Id")),
                                FechaUtc = DateTime.SpecifyKind(lector.GetDateTime(lector.GetOrdinal("FechaUtc")), DateTimeKind.Utc),
                                UsuarioId = LeerEnteroNulo(lector, "UsuarioId"),
                                NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario")),
                                Accion = lector.GetString(lector.GetOrdinal("Accion")),
                                Detalle = LeerTextoNulo(lector, "Detalle"),
                                Entidad = LeerTextoNulo(lector, "Entidad"),
                                EntidadId = LeerEnteroNulo(lector, "EntidadId"),
                                ValorAnterior = LeerTextoNulo(lector, "ValorAnterior"),
                                ValorNuevo = LeerTextoNulo(lector, "ValorNuevo")
                            });
                        }
                    }
                }
                return lista;
            });
        }

        private static void AgregarParametros(SqlCommand comando, RegistroAuditoria r)
        {
            comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = r.UsuarioId.HasValue ? r.UsuarioId.Value : DBNull.Value;
            comando.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 50).Value = AccionAuditoria.Truncar(r.NombreUsuario, 50) ?? string.Empty;
            comando.Parameters.Add("@Accion", SqlDbType.NVarChar, 40).Value = r.Accion;
            comando.Parameters.Add("@Detalle", SqlDbType.NVarChar, 500).Value = (object?)AccionAuditoria.Truncar(r.Detalle, 500) ?? DBNull.Value;
            comando.Parameters.Add("@Entidad", SqlDbType.NVarChar, 50).Value = (object?)r.Entidad ?? DBNull.Value;
            comando.Parameters.Add("@EntidadId", SqlDbType.Int).Value = r.EntidadId.HasValue ? r.EntidadId.Value : DBNull.Value;
            comando.Parameters.Add("@ValorAnterior", SqlDbType.NVarChar, 100).Value = (object?)AccionAuditoria.Truncar(r.ValorAnterior, 100) ?? DBNull.Value;
            comando.Parameters.Add("@ValorNuevo", SqlDbType.NVarChar, 100).Value = (object?)AccionAuditoria.Truncar(r.ValorNuevo, 100) ?? DBNull.Value;
        }

        private static string? LeerTextoNulo(SqlDataReader lector, string columna)
        {
            int i = lector.GetOrdinal(columna);
            return lector.IsDBNull(i) ? null : lector.GetString(i);
        }

        private static int? LeerEnteroNulo(SqlDataReader lector, string columna)
        {
            int i = lector.GetOrdinal(columna);
            return lector.IsDBNull(i) ? (int?)null : lector.GetInt32(i);
        }
    }
}