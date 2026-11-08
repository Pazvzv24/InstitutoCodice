using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Menú principal. Gestión solo para administrador.
    /// Cada pantalla se crea con el Contenedor, que le entrega sus repositorios.
    ///
    /// Las pantallas de trabajo se abren como ventanas independientes (no modales):
    /// se pueden tener varias abiertas a la vez, pero solo una de cada tipo.
    /// Cada una aparece en la barra de tareas de Windows para pasar de una a otra.
    /// Al cerrar el menú (o cerrar sesión) se cierran todas, así el siguiente
    /// usuario no puede ver datos de la sesión anterior.
    /// </summary>
    public partial class FormPrincipal : Form
    {
        // Ventanas abiertas, una por tipo de pantalla
        private readonly Dictionary<Type, Form> abiertas = new Dictionary<Type, Form>();

        public FormPrincipal()
        {
            InitializeComponent();
            FormClosing += (s, e) => CerrarVentanasAbiertas();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = "Bienvenido(a), " + Sesion.UsuarioActual.Nombre;
            lblRol.Text = "Rol: " + Sesion.UsuarioActual.ObtenerRol();
            grpGestion.Enabled = Sesion.EsAdministrador;
        }

        /// <summary>
        /// Abre una pantalla de trabajo. Si ya está abierta, la trae al frente
        /// en vez de abrir una segunda copia.
        /// </summary>
        private void Abrir<T>() where T : Form
        {
            if (abiertas.TryGetValue(typeof(T), out Form? existente) && !existente.IsDisposed)
            {
                if (existente.WindowState == FormWindowState.Minimized)
                    existente.WindowState = FormWindowState.Normal;
                existente.Activate();
                return;
            }

            T formulario = Contenedor.Crear<T>();
            formulario.ShowInTaskbar = true; // cada ventana aparece en la barra de tareas
            formulario.FormClosed += (s, e) => abiertas.Remove(typeof(T));
            abiertas[typeof(T)] = formulario;
            formulario.Show();
        }

        /// <summary>
        /// Cierra todas las pantallas abiertas. Se llama al cerrar el menú o la sesión:
        /// ninguna ventana con datos queda abierta para el siguiente usuario.
        /// </summary>
        private void CerrarVentanasAbiertas()
        {
            foreach (Form ventana in new List<Form>(abiertas.Values))
            {
                if (!ventana.IsDisposed)
                    ventana.Close();
            }
            abiertas.Clear();
        }

        /// <summary>
        /// Abre un diálogo que se debe completar antes de seguir (modal).
        /// </summary>
        private void AbrirDialogo(Form formulario)
        {
            using (formulario)
            {
                formulario.ShowDialog(this);
            }
        }

        private void btnEstudiantes_Click(object sender, EventArgs e) => Abrir<FormEstudiantes>();
        private void btnDocentes_Click(object sender, EventArgs e) => Abrir<FormDocentes>();
        private void btnAsignaturas_Click(object sender, EventArgs e) => Abrir<FormAsignaturas>();
        private void btnInscripciones_Click(object sender, EventArgs e) => Abrir<FormInscripciones>();
        private void btnAsistencia_Click(object sender, EventArgs e) => Abrir<FormAsistencia>();
        private void btnCalificaciones_Click(object sender, EventArgs e) => Abrir<FormCalificaciones>();
        private void btnInformes_Click(object sender, EventArgs e) => Abrir<FormInformes>();
        private void btnAuditoria_Click(object sender, EventArgs e) => Abrir<FormAuditoria>();

        // Cambiar la contraseña sigue siendo un diálogo modal: hay que terminarlo o cancelarlo
        private void btnCambiarContrasena_Click(object sender, EventArgs e) =>
            AbrirDialogo(Contenedor.Crear<FormCambiarContrasena>(false)); // false = cambio voluntario

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Bitacora.Registrar(AccionAuditoria.CierreSesion);
            Sesion.CerroSesion = true;
            Close(); // al cerrarse, el menú cierra también todas las ventanas abiertas
        }
    }
}