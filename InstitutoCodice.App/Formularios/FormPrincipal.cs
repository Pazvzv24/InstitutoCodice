using System;
using System.Windows.Forms;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Menú principal. Gestión solo para administrador.
    /// Cada pantalla se crea con el Contenedor, que le entrega sus repositorios.
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

        private void btnEstudiantes_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormEstudiantes>());
        private void btnDocentes_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormDocentes>());
        private void btnAsignaturas_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormAsignaturas>());
        private void btnInscripciones_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormInscripciones>());
        private void btnAsistencia_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormAsistencia>());
        private void btnCalificaciones_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormCalificaciones>());
        private void btnInformes_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormInformes>());

        private void btnAuditoria_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormAuditoria>());

        private void btnCambiarContrasena_Click(object sender, EventArgs e) => Abrir(Contenedor.Crear<FormCambiarContrasena>(false)); // false = cambio voluntario

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Bitacora.Registrar(AccionAuditoria.CierreSesion);
            Sesion.CerroSesion = true;
            Close();
        }
    }
}