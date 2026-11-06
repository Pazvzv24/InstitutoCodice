using System;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Login de usuarios (RF-12).
    /// - La contraseña distingue mayúsculas (se compara por hash).
    /// - Al 3er intento fallido la aplicación se cierra.
    /// </summary>
    public partial class FormLogin : Form
    {
        private readonly UsuarioRepositorio usuarioRepositorio = new UsuarioRepositorio();

        // NUEVO: control de intentos fallidos
        private const int MaxIntentos = 3;
        private int intentosFallidos = 0;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string clave = txtContrasena.Text; // Sin Trim/ToLower: distingue mayúsculas

            if (nombreUsuario.Length == 0 || clave.Length == 0)
            {
                Mensajes.Advertencia("Ingrese su usuario y contraseña.");
                return;
            }

            Usuario usuario;
            try
            {
                usuario = usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);
            }
            catch (Exception ex)
            {
                // Un error de conexión no cuenta como intento fallido
                Mensajes.Error(ex);
                return;
            }

            if (usuario == null || !usuario.Activo || !usuario.VerificarContrasena(clave))
            {
                // NUEVO: registrar intento fallido
                intentosFallidos++;

                if (intentosFallidos >= MaxIntentos)
                {
                    Mensajes.Advertencia("Se superó el número máximo de intentos (3). La aplicación se cerrará.");
                    Application.Exit();
                    return;
                }

                int restantes = MaxIntentos - intentosFallidos;
                Mensajes.Advertencia($"Usuario o contraseña incorrectos. Intentos restantes: {restantes}.");
                txtContrasena.Clear();
                txtContrasena.Focus();
                return;
            }

            // NUEVO: login correcto, se reinicia el contador
            intentosFallidos = 0;

            Sesion.Iniciar(usuario);
            Hide();
            using (var principal = new FormPrincipal())
            {
                principal.ShowDialog();
            }

            if (Sesion.CerroSesion)
            {
                Sesion.Cerrar();
                txtContrasena.Clear();
                Show();
                txtUsuario.Focus();
            }
            else
            {
                Close();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}