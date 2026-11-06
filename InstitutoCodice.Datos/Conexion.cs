using System.IO;
using Microsoft.Data.SqlClient;

namespace InstitutoCodice.Datos
{
    /// <summary>
    /// Administra la conexión hacia la instancia de SQL Server del sistema.
    ///
    /// Dos modos de autenticación (se eligen en appsettings.json):
    /// - "Windows": usa la cuenta de Windows de quien ejecuta la app. Para instalar y administrar.
    /// - "Sql": usa la cuenta de la aplicación (app_codice) con permisos mínimos (rol_app_codice).
    ///   La contraseña NO está en el código ni en appsettings: se lee desde una variable de entorno.
    ///
    /// Cifrado en tránsito: la conexión siempre va cifrada (TLS).
    /// - Con "CertificadoServidor" configurado: la app solo acepta ese certificado, así comprueba
    ///   que se conecta al servidor verdadero (evita ataques "man in the middle").
    /// - Sin él (modo desarrollo): acepta el certificado autogenerado de SQL Server sin verificarlo.
    ///   Ver docs/CertificadoSQL.md para configurarlo.
    /// </summary>
    public static class Conexion
    {
        public const string AutenticacionWindows = "Windows";
        public const string AutenticacionSql = "Sql";

        public static string Servidor { get; private set; } = @"localhost\SQLEXPRESS";
        public static string NombreBaseDatos { get; private set; } = "InstitutoCodiceDB";
        public static bool UsaAutenticacionWindows { get; private set; } = true;
        public static string? UsuarioSql { get; private set; }

        /// <summary>
        /// true si la app verifica el certificado del servidor (modo producción).
        /// </summary>
        public static bool VerificaCertificado => certificadoServidor != null;

        private static string? contrasenaSql;
        private static string? certificadoServidor;

        /// <summary>
        /// Cadena de conexión hacia la base de datos de la aplicación.
        /// </summary>
        public static string Cadena => ConstruirCadena(NombreBaseDatos);

        /// <summary>
        /// Define el servidor y la base a usar con autenticación de Windows.
        /// </summary>
        public static void Configurar(string? servidor, string? nombreBaseDatos)
        {
            Configurar(servidor, nombreBaseDatos, AutenticacionWindows, null, null);
        }

        /// <summary>
        /// Define el servidor, la base y el modo de autenticación. Se llama una vez al iniciar la app.
        /// Con autenticación "Sql", la contraseña se lee de la variable de entorno indicada.
        /// Con "certificadoServidor" (ruta a un archivo .cer), solo se acepta ese certificado.
        /// </summary>
        public static void Configurar(string? servidor, string? nombreBaseDatos,
                                      string? autenticacion, string? usuarioSql, string? variableContrasena,
                                      string? certificadoServidor = null)
        {
            Conexion.certificadoServidor = null;
            if (!string.IsNullOrWhiteSpace(certificadoServidor))
            {
                string ruta = certificadoServidor.Trim();
                if (!File.Exists(ruta))
                    throw new InvalidOperationException("No se encontró el certificado del servidor indicado en appsettings "
                        + "(\"CertificadoServidor\"): " + ruta + "\nRevise la ruta o deje el valor vacío en modo desarrollo.");
                Conexion.certificadoServidor = ruta;
            }

            if (!string.IsNullOrWhiteSpace(servidor))
                Servidor = servidor.Trim();

            if (!string.IsNullOrWhiteSpace(nombreBaseDatos))
                NombreBaseDatos = nombreBaseDatos.Trim();

            UsaAutenticacionWindows = !string.Equals(autenticacion?.Trim(), AutenticacionSql, StringComparison.OrdinalIgnoreCase);
            UsuarioSql = null;
            contrasenaSql = null;

            if (UsaAutenticacionWindows)
                return;

            if (string.IsNullOrWhiteSpace(usuarioSql))
                throw new InvalidOperationException("Con autenticación \"Sql\" debe indicar el \"Usuario\" en appsettings.");

            if (string.IsNullOrWhiteSpace(variableContrasena))
                throw new InvalidOperationException("Con autenticación \"Sql\" debe indicar \"VariableContrasena\" en appsettings.");

            string? valor = Environment.GetEnvironmentVariable(variableContrasena.Trim());
            if (string.IsNullOrEmpty(valor))
                throw new InvalidOperationException("No se encontró la variable de entorno \"" + variableContrasena.Trim()
                    + "\" con la contraseña del usuario " + usuarioSql.Trim()
                    + ".\nCréela en Windows y vuelva a abrir la aplicación.");

            UsuarioSql = usuarioSql.Trim();
            contrasenaSql = valor;
        }

        public static SqlConnection CrearConexion()
        {
            return new SqlConnection(Cadena);
        }

        /// <summary>
        /// Conexión a la base "master" del servidor, usada solo para verificar o crear la base de la app.
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
                Encrypt = SqlConnectionEncryptOption.Mandatory, // siempre cifrada
                ConnectTimeout = 5,
                ApplicationName = "InstitutoCodice",
                PersistSecurityInfo = false // la contraseña no queda legible en la conexión abierta
            };

            if (certificadoServidor != null)
            {
                // Producción: solo se acepta el certificado indicado (fijación de certificado)
                builder.TrustServerCertificate = false;
                builder.ServerCertificate = certificadoServidor;
            }
            else
            {
                // Desarrollo: certificado autogenerado de SQL Server, cifrado pero sin verificar (riesgo aceptado)
                builder.TrustServerCertificate = true;
            }

            if (UsaAutenticacionWindows)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = UsuarioSql;
                builder.Password = contrasenaSql;
            }

            return builder.ConnectionString;
        }
    }
}