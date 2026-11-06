using Microsoft.Data.SqlClient;

namespace InstitutoCodice.Datos
{
    /// <summary>
    /// Administra la conexión hacia la instancia de SQL Server del sistema.
    /// El servidor y la base se configuran en appsettings.json (no en el código),
    /// usando autenticación de Windows: no se guardan contraseñas.
    /// </summary>
    public static class Conexion
    {
        public static string Servidor { get; private set; } = @"localhost\SQLEXPRESS";
        public static string NombreBaseDatos { get; private set; } = "InstitutoCodiceDB";

        /// <summary>
        /// Cadena de conexión hacia la base de datos de la aplicación.
        /// </summary>
        public static string Cadena => ConstruirCadena(NombreBaseDatos);

        /// <summary>
        /// Define el servidor y la base a usar. Se llama una vez al iniciar la app.
        /// </summary>
        public static void Configurar(string? servidor, string? nombreBaseDatos)
        {
            if (!string.IsNullOrWhiteSpace(servidor))
                Servidor = servidor.Trim();

            if (!string.IsNullOrWhiteSpace(nombreBaseDatos))
                NombreBaseDatos = nombreBaseDatos.Trim();
        }

        public static SqlConnection CrearConexion()
        {
            return new SqlConnection(Cadena);
        }

        /// <summary>
        /// Conexión a la base "master" del servidor, usada solo para crear la base de la app.
        /// </summary>
        public static SqlConnection CrearConexionServidor()
        {
            return new SqlConnection(ConstruirCadena("master"));
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

        private static string ConstruirCadena(string baseDatos)
        {
            // SqlConnectionStringBuilder evita errores de formato e inyección en la cadena
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Servidor,
                InitialCatalog = baseDatos,
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                ConnectTimeout = 5
            };
            return builder.ConnectionString;
        }
    }
}