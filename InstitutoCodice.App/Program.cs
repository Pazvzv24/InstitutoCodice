using InstitutoCodice.App.Formularios;
using InstitutoCodice.Datos;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

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
        /// Lee appsettings.json, configura la conexión y crea la base de demostración
        /// si no existe. Devuelve false si la app no puede continuar.
        /// </summary>
        private static bool PrepararBaseDatos()
        {
            IConfiguration config;
            try
            {
                config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Mensajes.Advertencia("El archivo appsettings.json tiene un formato inválido.\n\nDetalle: " + ex.Message);
                return false;
            }

            Conexion.Configurar(config["BaseDatos:Servidor"], config["BaseDatos:Nombre"]);

            bool crearSiNoExiste = !bool.TryParse(config["BaseDatos:CrearSiNoExiste"], out bool valor) || valor;

            try
            {
                if (crearSiNoExiste && !InstaladorBaseDatos.ExisteBaseDatos())
                {
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
                Mensajes.Advertencia(error + "\n\nRevise el valor \"Servidor\" en appsettings.json.");
                return false;
            }

            return true;
        }
    }
}