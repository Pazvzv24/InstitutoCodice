using System;
using System.Windows.Forms;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Muestra el aviso de privacidad (Ley 21.719).
    /// Se puede abrir desde el login, antes de ingresar.
    /// </summary>
    public partial class FormAvisoPrivacidad : Form
    {
        public FormAvisoPrivacidad()
        {
            InitializeComponent();
        }

        private void FormAvisoPrivacidad_Load(object sender, EventArgs e)
        {
            Text = "Instituto Códice - " + AvisoPrivacidad.Titulo;
            lblTitulo.Text = AvisoPrivacidad.Titulo;
            txtAviso.Text = AvisoPrivacidad.Texto;

            // Que se lea desde el inicio, sin texto seleccionado
            txtAviso.SelectionStart = 0;
            txtAviso.SelectionLength = 0;
            btnCerrar.Focus();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}