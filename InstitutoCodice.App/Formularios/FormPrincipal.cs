using System;
using System.Windows.Forms;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Menú principal. Gestión solo para administrador.
    /// </summary>
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = "Bienvenido(a), " + Sesion.UsuarioActual.Nombre;
            lblRol.Text = "Rol: " + Sesion.UsuarioActual.ObtenerRol();
            grpGestion.Enabled = Sesion.EsAdministrador;
        }

        private void Abrir(Form formulario)
        {
            using (formulario)
            {
                formulario.ShowDialog(this);
            }
        }

        private void btnEstudiantes_Click(object sender, EventArgs e) => Abrir(new FormEstudiantes());
        private void btnDocentes_Click(object sender, EventArgs e) => Abrir(new FormDocentes());
        private void btnAsignaturas_Click(object sender, EventArgs e) => Abrir(new FormAsignaturas());
        private void btnInscripciones_Click(object sender, EventArgs e) => Abrir(new FormInscripciones());
        private void btnAsistencia_Click(object sender, EventArgs e) => Abrir(new FormAsistencia());
        private void btnCalificaciones_Click(object sender, EventArgs e) => Abrir(new FormCalificaciones());
        private void btnInformes_Click(object sender, EventArgs e) => Abrir(new FormInformes());

        private void btnCambiarContrasena_Click(object sender, EventArgs e) => Abrir(new FormCambiarContrasena(obligatorio: false));

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Sesion.CerroSesion = true;
            Close();
        }
    }
}