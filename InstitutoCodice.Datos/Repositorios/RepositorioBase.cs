using System;
using Microsoft.Data.SqlClient;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Clase base para los repositorios: gestiona la conexión y el manejo centralizado de errores.
    /// </summary>
    public abstract class RepositorioBase
    {
        protected T Ejecutar<T>(Func<SqlConnection, T> operacion)
        {
            try
            {
                using (SqlConnection conexion = Conexion.CrearConexion())
                {
                    conexion.Open();
                    return operacion(conexion);
                }
            }
            catch (SqlException ex)
            {
                throw TraducirError(ex);
            }
        }

        protected void Ejecutar(Action<SqlConnection> operacion)
        {
            Ejecutar<bool>(conexion =>
            {
                operacion(conexion);
                return true;
            });
        }

        /// <summary>
        /// Ejecuta una operación dentro de una transacción para garantizar la atomicidad.
        /// </summary>
        protected void EjecutarEnTransaccion(Action<SqlConnection, SqlTransaction> operacion)
        {
            Ejecutar(conexion =>
            {
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        operacion(conexion, transaccion);
                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            });
        }

        protected static object ValorONulo(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? (object)DBNull.Value : valor.Trim();
        }

        protected static string LeerTexto(SqlDataReader lector, string columna)
        {
            int indice = lector.GetOrdinal(columna);
            return lector.IsDBNull(indice) ? "" : lector.GetString(indice);
        }

        private static DatosException TraducirError(SqlException ex)
        {
            string mensaje;
            switch (ex.Number)
            {
                case 2627:
                case 2601:
                    mensaje = "Ya existe un registro con esos datos (por ejemplo, el mismo RUT, código o usuario).";
                    break;
                case 547:
                    mensaje = "La operación no se puede realizar: el registro está relacionado con otros datos "
                            + "o un valor no cumple las reglas de la base de datos (por ejemplo, una fecha futura).";
                    break;
                case 4060:
                    mensaje = "No se encontró la base de datos InstitutoCodiceDB. Ejecuta primero el script de creación en SSMS.";
                    break;
                case 18456:
                    mensaje = "SQL Server rechazó el inicio de sesión. Revisa la cadena de conexión en Conexion.cs.";
                    break;
                case -2:
                case -1:
                case 2:
                case 53:
                case 26:
                case 40:
                    mensaje = "No se pudo conectar a SQL Server. Verifica que el servicio esté en ejecución "
                            + "y que el nombre del servidor en Conexion.cs sea correcto.";
                    break;
                default:
                    mensaje = "Error de base de datos: " + ex.Message;
                    break;
            }

            return new DatosException(mensaje, ex);
        }
    }
}
