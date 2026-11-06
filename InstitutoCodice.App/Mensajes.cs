using InstitutoCodice.Datos;
using System;
using System.Data;
using System.Windows.Forms;

namespace InstitutoCodice.App
{
    /// <summary>
    /// Centraliza los mensajes al usuario para mantener formato consistente.
    /// </summary>
    public static class Mensajes
    {
        private const string Titulo = "Instituto Códice";

        public static void Info(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Advertencia(string mensaje)
        {
            MessageBox.Show(mensaje, Titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool Confirmar(string pregunta)
        {
            return MessageBox.Show(pregunta, Titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// Muestra el error en pantalla sin detener la aplicación. Si es de validación o BD, usa el mensaje original.
        /// </summary>
        public static void Error(Exception ex)
        {
            string mensaje;
            if (ex is DatosException || ex is ArgumentException || ex is InvalidOperationException)
                mensaje = ex.Message;
            else
                mensaje = "Ocurrió un error inesperado: " + ex.Message;

            MessageBox.Show(mensaje, Titulo + " - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}