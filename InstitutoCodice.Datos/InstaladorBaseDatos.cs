using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace InstitutoCodice.Datos
{
    /// <summary>
    /// Crea la base de datos y carga los datos de demostración la primera vez
    /// que se abre la aplicación, ejecutando los scripts incluidos con el programa.
    ///
    /// Seguridad:
    /// - Solo actúa si la base NO existe: nunca modifica ni borra una base existente.
    /// - Solo ejecuta los scripts que vienen junto al ejecutable (carpeta BaseDatos).
    /// </summary>
    public static class InstaladorBaseDatos
    {
        private static readonly string[] Scripts =
        {
            "01_InstitutoCodiceDB.sql",
            "02_DatosEjemplo.sql"
        };

        // Separa el script en lotes por cada línea que contenga solo "GO" (como hace SSMS)
        private static readonly Regex SeparadorGo =
            new Regex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Indica si la base de datos de la aplicación ya existe en el servidor.
        /// </summary>
        public static bool ExisteBaseDatos()
        {
            using (SqlConnection cn = Conexion.CrearConexionServidor())
            using (SqlCommand cmd = new SqlCommand("SELECT DB_ID(@nombre)", cn))
            {
                cmd.Parameters.AddWithValue("@nombre", Conexion.NombreBaseDatos);
                cn.Open();
                object? resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value;
            }
        }

        /// <summary>
        /// Ejecuta los scripts de instalación en orden.
        /// </summary>
        /// <param name="carpetaScripts">Carpeta donde están los .sql (normalmente BaseDatos junto al .exe).</param>
        public static void Instalar(string carpetaScripts)
        {
            if (ExisteBaseDatos())
                return; // Protección: nunca se reinstala sobre una base existente

            using (SqlConnection cn = Conexion.CrearConexionServidor())
            {
                cn.Open();
                foreach (string nombre in Scripts)
                {
                    string ruta = Path.Combine(carpetaScripts, nombre);
                    if (!File.Exists(ruta))
                        throw new FileNotFoundException("No se encontró el script de instalación.", ruta);

                    EjecutarScript(cn, File.ReadAllText(ruta));
                }
            }
        }

        private static void EjecutarScript(SqlConnection cn, string script)
        {
            foreach (string lote in SeparadorGo.Split(script))
            {
                if (string.IsNullOrWhiteSpace(lote))
                    continue;

                using (SqlCommand cmd = new SqlCommand(lote, cn))
                {
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}