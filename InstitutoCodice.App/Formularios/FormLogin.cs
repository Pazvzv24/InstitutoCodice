using System;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Login de usuarios (RF-12).
    /// - La contraseña distingue mayúsculas (se compara por hash).
    /// - Al 3er intento fallido en la sesión, la aplicación se cierra.
    /// - Al 3er intento fallido de una cuenta, esta se bloquea 15 minutos en la base de datos
    ///   (el bloqueo se mantiene aunque se reinicie la aplicación).
    /// - Si la cuenta tiene una contraseña asignada por el administrador, se exige cambiarla.
    /// - Cada ingreso, fallo y bloqueo queda en la bitácora de auditoría (nunca la contraseña).
    /// - Enlace al aviso de privacidad (Ley 21.719), disponible antes de ingresar.
    /// </summary>
    public partial class FormLogin : Form
    {
        private readonly UsuarioRepositorio usuarioRepositorio; // llega por el constructor

        // Control de intentos fallidos en esta sesión de la aplicación
        private const int MaxIntentos = 3;
        private int intentosFallidos = 0;

        public FormLogin(UsuarioRepositorio usuarioRepositorio)
        {
            InitializeComponent();
            this.usuarioRepositorio = usuarioRepositorio;
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

            DateTime ahora = DateTime.UtcNow;
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

            // Cuenta bloqueada: no se verifica la contraseña mientras dure el bloqueo
            if (usuario != null && PoliticaBloqueo.EstaBloqueado(usuario, ahora))
            {
                int minutos = PoliticaBloqueo.MinutosRestantes(usuario, ahora);
                Bitacora.Registrar(AccionAuditoria.LoginBloqueado, "Intento de ingreso con la cuenta bloqueada.", usuario: usuario);
                Mensajes.Advertencia($"Cuenta bloqueada temporalmente por intentos fallidos. Intente nuevamente en {minutos} minuto(s).");
                txtContrasena.Clear();
                return;
            }

            if (usuario == null || !usuario.Activo || !usuario.VerificarContrasena(clave))
            {
                bool quedoBloqueada = false;

                // Se registra el fallo en la cuenta (si existe)
                if (usuario != null)
                {
                    try
                    {
                        quedoBloqueada = PoliticaBloqueo.RegistrarFallo(usuario, ahora);
                        usuarioRepositorio.ActualizarEstadoAcceso(usuario);
                    }
                    catch (Exception ex)
                    {
                        Mensajes.Error(ex);
                        return;
                    }
                }

                Bitacora.Registrar(AccionAuditoria.LoginFallido,
                    usuario == null ? "Usuario inexistente." : (!usuario.Activo ? "Cuenta desactivada." : "Contraseña incorrecta."),
                    usuario: usuario, nombreUsuario: nombreUsuario);
                if (quedoBloqueada)
                    Bitacora.Registrar(AccionAuditoria.CuentaBloqueada,
                        $"Cuenta bloqueada {PoliticaBloqueo.DuracionBloqueo.TotalMinutes} minutos por {PoliticaBloqueo.MaxIntentos} intentos fallidos.",
                        usuario: usuario);

                intentosFallidos++;

                if (intentosFallidos >= MaxIntentos)
                {
                    Mensajes.Advertencia("Se superó el número máximo de intentos (3). La aplicación se cerrará.");
                    Application.Exit();
                    return;
                }

                if (quedoBloqueada)
                {
                    Mensajes.Advertencia($"Cuenta bloqueada por {PoliticaBloqueo.DuracionBloqueo.TotalMinutes} minutos debido a intentos fallidos.");
                }
                else
                {
                    int restantes = MaxIntentos - intentosFallidos;
                    Mensajes.Advertencia($"Usuario o contraseña incorrectos. Intentos restantes: {restantes}.");
                }

                txtContrasena.Clear();
                txtContrasena.Focus();
                return;
            }

            // Login correcto: se reinician los contadores
            intentosFallidos = 0;
            if (usuario.IntentosFallidos > 0 || usuario.BloqueadoHasta.HasValue)
            {
                try
                {
                    PoliticaBloqueo.RegistrarExito(usuario);
                    usuarioRepositorio.ActualizarEstadoAcceso(usuario);
                }
                catch (Exception ex)
                {
                    Mensajes.Error(ex);
                    return;
                }
            }

            Sesion.Iniciar(usuario);
            Bitacora.Registrar(AccionAuditoria.LoginExitoso);

            // Cambio de contraseña obligatorio (por ejemplo, contraseña asignada por el administrador)
            if (usuario.DebeCambiarContrasena)
            {
                Mensajes.Info("Por seguridad, debe cambiar su contraseña antes de continuar.");
                using (var cambio = Contenedor.Crear<FormCambiarContrasena>(true)) // true = cambio obligatorio
                {
                    if (cambio.ShowDialog(this) != DialogResult.OK)
                    {
                        Sesion.Cerrar();
                        txtContrasena.Clear();
                        txtUsuario.Focus();
                        return;
                    }
                }
            }

            Hide();
            using (var principal = Contenedor.Crear<FormPrincipal>())
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

        // Ley 21.719: el aviso se puede leer antes de ingresar
        private void lnkPrivacidad_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var aviso = Contenedor.Crear<FormAvisoPrivacidad>())
            {
                aviso.ShowDialog(this);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}