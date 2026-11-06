using System;
using System.Windows.Forms;
using InstitutoCodice.App.Formularios;

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

            Application.Run(new FormLogin());
        }
    }
}