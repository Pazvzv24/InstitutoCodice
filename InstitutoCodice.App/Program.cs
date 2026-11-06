using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using InstitutoCodice.App.Formularios;
using InstitutoCodice.Datos;

namespace InstitutoCodice.App
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal de la aplicación WinForms.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cualquier excepción no controlada se captura para no cerrar la app de golpe.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => Mensajes.Error(e.Exception);

            if (!PrepararBaseDatos())
                return;

            Application.Run(new FormLogin());
        }

        /// <summary>
        /// Lee la configuración, configura la conexión y crea la base de demostración
        /// si no existe. Devuelve false si la app no puede continuar.
        ///
        /// Configuración:
        /// - appsettings.json: valores por defecto (van al repositorio).
        /// - appsettings.local.json: opcional, sobrescribe lo anterior en este equipo
        ///   (está en .gitignore y nunca se sube).
        /// </summary>
        private static bool PrepararBaseDatos()
        {
            IConfiguration config;
            try
            {
                config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.local.json", optional: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Mensajes.Advertencia("El archivo de configuración tiene un formato inválido.\n\nDetalle: " + ex.Message);
                return false;
            }

            try
            {
                Conexion.Configurar(
                    config["BaseDatos:Servidor"],
                    config["BaseDatos:Nombre"],
                    config["BaseDatos:Autenticacion"],
                    config["BaseDatos:Usuario"],
                    config["BaseDatos:VariableContrasena"]);
            }
            catch (InvalidOperationException ex)
            {
                Mensajes.Advertencia(ex.Message);
                return false;
            }

            bool crearSiNoExiste = !bool.TryParse(config["BaseDatos:CrearSiNoExiste"], out bool valor) || valor;

            try
            {
                if (!InstaladorBaseDatos.ExisteBaseDatos())
                {
                    // Separación de funciones: la cuenta de la app (mínimo privilegio) no puede crear bases.
                    if (!Conexion.UsaAutenticacionWindows)
                    {
                        Mensajes.Advertencia("La base de datos \"" + Conexion.NombreBaseDatos + "\" no existe.\n\n"
                            + "Debe instalarla un administrador con \"Autenticacion\": \"Windows\" "
                            + "antes de usar la cuenta de la aplicación (" + Conexion.UsuarioSql + ").");
                        return false;
                    }

                    if (!crearSiNoExiste)
                    {
                        Mensajes.Advertencia("La base de datos \"" + Conexion.NombreBaseDatos + "\" no existe en " + Conexion.Servidor + ".");
                        return false;
                    }

                    DialogResult respuesta = MessageBox.Show(
                        "No se encontró la base de datos \"" + Conexion.NombreBaseDatos + "\" en el servidor "
                        + Conexion.Servidor + ".\n\n¿Desea crearla con los datos de demostración?",
                        "Primera ejecución", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (respuesta != DialogResult.Yes)
                        return false;

                    Cursor.Current = Cursors.WaitCursor;
                    InstaladorBaseDatos.Instalar(Path.Combine(AppContext.BaseDirectory, "BaseDatos"));
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show("Base de datos creada correctamente.\n\nPuede ingresar con admin / Admin123.",
                        "Instalación completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Mensajes.Advertencia("No se pudo preparar la base de datos en el servidor " + Conexion.Servidor + ".\n\n"
                    + "Revise el valor \"Servidor\" en appsettings.json (junto al ejecutable).\n"
                    + "Ejemplos: localhost\\SQLEXPRESS  o  (localdb)\\MSSQLLocalDB\n\nDetalle: " + ex.Message);
                return false;
            }

            if (!Conexion.Probar(out string error))
            {
                Mensajes.Advertencia(error + "\n\nRevise la configuración de la base de datos en appsettings.json.");
                return false;
            }

            return true;
        }
    }
}