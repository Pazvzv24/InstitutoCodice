using System;
using System.Text;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Cambio de la contraseña del usuario conectado (RNF-03).
    /// - Pide la contraseña actual (confirma que es el dueño de la sesión).
    /// - La nueva debe cumplir la PoliticaContrasena y escribirse dos veces.
    /// - En modo obligatorio no se puede omitir: cancelar vuelve al login.
    /// </summary>
    public partial class FormCambiarContrasena : Form
    {
        private readonly UsuarioRepositorio repositorio = new UsuarioRepositorio();
        private readonly bool obligatorio;

        public FormCambiarContrasena(bool obligatorio)
        {
            InitializeComponent();
            this.obligatorio = obligatorio;
        }

        private void FormCambiarContrasena_Load(object sender, EventArgs e)
        {
            lblReglas.Text = PoliticaContrasena.Descripcion;

            if (obligatorio)
            {
                lblTitulo.Text = "Debe cambiar su contraseña";
                btnCancelar.Text = "Volver al login";
            }

            txtActual.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var usuario = Sesion.UsuarioActual;
            string actual = txtActual.Text;
            string nueva = txtNueva.Text;

            if (!usuario.VerificarContrasena(actual))
            {
                Mensajes.Advertencia("La contraseña actual no es correcta.");
                txtActual.Clear();
                txtActual.Focus();
                return;
            }

            var errores = new StringBuilder();
            foreach (string error in PoliticaContrasena.Validar(nueva, usuario.NombreUsuario, actual))
                errores.AppendLine("- " + error);
            if (nueva != txtConfirmar.Text)
                errores.AppendLine("- La confirmación no coincide con la nueva contraseña.");

            if (errores.Length > 0)
            {
                Mensajes.Advertencia("Revise la nueva contraseña:\n" + errores);
                txtNueva.Clear();
                txtConfirmar.Clear();
                txtNueva.Focus();
                return;
            }

            try
            {
                string nuevoHash = Seguridad.GenerarHash(nueva);
                repositorio.CambiarContrasena(usuario.Id, nuevoHash);

                usuario.ContrasenaHash = nuevoHash;
                usuario.DebeCambiarContrasena = false;
                Bitacora.Registrar(AccionAuditoria.CambioContrasena,
                    obligatorio ? "Cambio obligatorio al ingresar." : "Cambio voluntario desde el menú.",
                    entidad: "Usuario", entidadId: usuario.Id);

                Mensajes.Info("Contraseña actualizada correctamente.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }
    }
}