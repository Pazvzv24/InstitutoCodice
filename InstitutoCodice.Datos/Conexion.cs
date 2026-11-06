using Microsoft.Data.SqlClient;

namespace InstitutoCodice.Datos
{
    /// <summary>
    /// Administra la conexión hacia la instancia de SQL Server del sistema.
    /// </summary>
    public static class Conexion
    {
        public static string Cadena =
           @"Server=localhost\SQLEXPRESS;Database=InstitutoCodiceDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5;";

        public static SqlConnection CrearConexion()
        {
            return new SqlConnection(Cadena);
        }

        /// <summary>
        /// Intenta abrir una conexión para verificar disponibilidad de la BD.
        /// </summary>
        public static bool Probar(out string error)
        {
            try
            {
                using (SqlConnection cn = CrearConexion())
                {
                    cn.Open();
                }
                error = string.Empty;
                return true;
            }
            catch (SqlException ex)
            {
                error = "No se pudo conectar a la base de datos.\n\nDetalle: " + ex.Message;
                return false;
            }
        }
    }
}