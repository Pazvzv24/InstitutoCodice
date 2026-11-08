using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Gestión de docentes (solo administrador).
    /// Las contraseñas asignadas deben cumplir la PoliticaContrasena.
    /// F5 o "Recargar" vuelve a leer la lista desde la base.
    /// </summary>
    public partial class FormDocentes : Form
    {
        private readonly DocenteRepositorio repositorio; // llega por el constructor
        private Docente? seleccionado; // null = modo "nuevo docente"

        public FormDocentes(DocenteRepositorio repositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            dgvDocentes.AutoGenerateColumns = false;
        }

        private void FormDocentes_Load(object sender, EventArgs e)
        {
            CargarDocentes();
            LimpiarFormulario();
        }

        private void CargarDocentes()
        {
            try
            {
                var docentes = repositorio.Listar();
                dgvDocentes.DataSource = null;
                dgvDocentes.DataSource = docentes;
                dgvDocentes.ClearSelection();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void dgvDocentes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var docente = dgvDocentes.Rows[e.RowIndex].DataBoundItem as Docente;
            if (docente == null) return;

            seleccionado = docente;
            txtNombre.Text = docente.Nombre;
            txtRut.Text = docente.Rut;
            txtCorreo.Text = docente.Correo;
            txtUsuario.Text = docente.NombreUsuario;
            txtContrasena.Clear();
            chkActivo.Checked = docente.Activo;
            lblModo.Text = "Editando: " + docente.Nombre;
        }

        private void LimpiarFormulario()
        {
            seleccionado = null;
            txtNombre.Clear();
            txtRut.Clear();
            txtCorreo.Clear();
            txtUsuario.Clear();
            txtContrasena.Clear();
            chkActivo.Checked = true;
            lblModo.Text = "Modo: nuevo docente";
            dgvDocentes.ClearSelection();
            txtNombre.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool esNuevo = seleccionado == null;
            string clave = txtContrasena.Text;

            var docente = new Docente
            {
                Id = seleccionado?.Id ?? 0,
                Nombre = txtNombre.Text.Trim(),
                Rut = ValidadorRut.Normalizar(txtRut.Text),
                Correo = txtCorreo.Text.Trim(),
                NombreUsuario = txtUsuario.Text.Trim(),
                Activo = chkActivo.Checked
            };

            var errores = new StringBuilder();
            if (docente.Nombre.Length == 0)
                errores.AppendLine("- El nombre es obligatorio.");
            if (!ValidadorRut.EsValido(docente.Rut))
                errores.AppendLine("- El RUT no es válido (ejemplo: 12345678-5).");
            if (!docente.Correo.Contains("@") || !docente.Correo.Contains("."))
                errores.AppendLine("- El correo no tiene un formato válido.");
            if (docente.NombreUsuario.Length == 0)
                errores.AppendLine("- El usuario es obligatorio.");
            if (esNuevo && clave.Length == 0)
                errores.AppendLine("- La contraseña es obligatoria para un docente nuevo.");
            if (clave.Length > 0)
            {
                foreach (string error in PoliticaContrasena.Validar(clave, docente.NombreUsuario))
                    errores.AppendLine("- Contraseña: " + error);
            }

            if (errores.Length > 0)
            {
                Mensajes.Advertencia("Revise los datos:\n" + errores);
                return;
            }

            try
            {
                if (clave.Length > 0)
                    docente.ContrasenaHash = Seguridad.GenerarHash(clave);

                if (esNuevo)
                {
                    repositorio.Agregar(docente);
                    Bitacora.Registrar(AccionAuditoria.UsuarioCreado, "Docente " + docente.NombreUsuario + " creado.",
                        entidad: "Usuario", entidadId: docente.Id);
                    Mensajes.Info("Docente registrado. Deberá cambiar su contraseña en el primer ingreso.");
                }
                else
                {
                    repositorio.Actualizar(docente, clave.Length > 0);

                    if (clave.Length > 0)
                        Bitacora.Registrar(AccionAuditoria.ContrasenaAsignada,
                            "Contraseña asignada por el administrador a " + docente.NombreUsuario + ".",
                            entidad: "Usuario", entidadId: docente.Id);
                    if (seleccionado != null && seleccionado.Activo && !docente.Activo)
                        Bitacora.Registrar(AccionAuditoria.UsuarioDesactivado, "Docente " + docente.NombreUsuario + " desactivado.",
                            entidad: "Usuario", entidadId: docente.Id, valorAnterior: "Activo", valorNuevo: "Inactivo");
                    else if (seleccionado != null && !seleccionado.Activo && docente.Activo)
                        Bitacora.Registrar(AccionAuditoria.UsuarioReactivado, "Docente " + docente.NombreUsuario + " reactivado.",
                            entidad: "Usuario", entidadId: docente.Id, valorAnterior: "Inactivo", valorNuevo: "Activo");
                    Mensajes.Info(clave.Length > 0
                        ? "Datos actualizados. El docente deberá cambiar su contraseña en el próximo ingreso."
                        : "Datos del docente actualizados.");
                }

                CargarDocentes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormDocentes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Recargar();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Vuelve a leer los datos desde la base (por ejemplo, si se modificaron en otra ventana).
        /// </summary>
        private void Recargar()
        {
            CargarDocentes();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}