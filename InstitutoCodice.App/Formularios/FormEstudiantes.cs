using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Gestión de estudiantes (RF-01, RF-02). Las desactivaciones quedan en la bitácora.
    /// </summary>
    public partial class FormEstudiantes : Form
    {
        private readonly EstudianteRepositorio repositorio; // llega por el constructor
        private List<Estudiante> estudiantes = new List<Estudiante>();
        private Estudiante seleccionado;

        public FormEstudiantes(EstudianteRepositorio repositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            dgvEstudiantes.AutoGenerateColumns = false;
        }

        private void FormEstudiantes_Load(object sender, EventArgs e)
        {
            CargarEstudiantes();
            LimpiarFormulario();
        }

        private void CargarEstudiantes()
        {
            try
            {
                estudiantes = repositorio.Listar();
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void AplicarFiltro()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            var lista = filtro.Length == 0
                ? estudiantes
                : estudiantes.Where(x => (x.Rut + " " + x.NombreCompleto() + " " + x.Curso).ToLower().Contains(filtro)).ToList();

            dgvEstudiantes.DataSource = null;
            dgvEstudiantes.DataSource = lista;
            dgvEstudiantes.ClearSelection();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void dgvEstudiantes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var estudiante = dgvEstudiantes.Rows[e.RowIndex].DataBoundItem as Estudiante;
            if (estudiante != null)
                MostrarEnFormulario(estudiante);
        }

        private void MostrarEnFormulario(Estudiante estudiante)
        {
            seleccionado = estudiante;
            txtRut.Text = estudiante.Rut;
            txtNombres.Text = estudiante.Nombres;
            txtApellidos.Text = estudiante.Apellidos;
            txtCorreo.Text = estudiante.Correo;
            txtCurso.Text = estudiante.Curso;
            chkActivo.Checked = estudiante.Activo;
            lblModo.Text = "Editando: " + estudiante.NombreCompleto();
        }

        private void LimpiarFormulario()
        {
            seleccionado = null;
            txtRut.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtCorreo.Clear();
            txtCurso.Clear();
            chkActivo.Checked = true;
            lblModo.Text = "Modo: nuevo estudiante";
            dgvEstudiantes.ClearSelection();
            txtRut.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var estudiante = new Estudiante
            {
                Id = seleccionado != null ? seleccionado.Id : 0,
                Rut = ValidadorRut.Normalizar(txtRut.Text),
                Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                Curso = txtCurso.Text.Trim(),
                Activo = chkActivo.Checked
            };

            string errores = Validar(estudiante);
            if (errores.Length > 0)
            {
                Mensajes.Advertencia("Revise los datos:\n" + errores);
                return;
            }

            try
            {
                if (estudiante.Id == 0)
                {
                    repositorio.Agregar(estudiante);
                    Mensajes.Info("Estudiante registrado correctamente.");
                }
                else
                {
                    repositorio.Actualizar(estudiante);
                    if (seleccionado != null && seleccionado.Activo && !estudiante.Activo)
                        Bitacora.Registrar(AccionAuditoria.EstudianteDesactivado, "Estudiante " + estudiante.Rut + " desactivado.",
                            entidad: "Estudiante", entidadId: estudiante.Id, valorAnterior: "Activo", valorNuevo: "Inactivo");
                    Mensajes.Info("Datos del estudiante actualizados.");
                }

                CargarEstudiantes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private static string Validar(Estudiante estudiante)
        {
            var errores = new StringBuilder();

            if (estudiante.Rut.Length == 0)
                errores.AppendLine("- El RUT es obligatorio.");
            else if (!estudiante.ValidarRut())
                errores.AppendLine("- El RUT no es válido (ejemplo: 12345678-5).");

            if (estudiante.Nombres.Length == 0)
                errores.AppendLine("- Los nombres son obligatorios.");
            if (estudiante.Apellidos.Length == 0)
                errores.AppendLine("- Los apellidos son obligatorios.");
            if (estudiante.Curso.Length == 0)
                errores.AppendLine("- El curso es obligatorio.");
            if (estudiante.Correo.Length > 0 && (!estudiante.Correo.Contains("@") || !estudiante.Correo.Contains(".")))
                errores.AppendLine("- El correo no tiene un formato válido.");

            return errores.ToString();
        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (seleccionado == null)
            {
                Mensajes.Advertencia("Seleccione un estudiante de la lista.");
                return;
            }

            if (!seleccionado.Activo)
            {
                Mensajes.Advertencia("El estudiante ya está inactivo.");
                return;
            }

            if (!Mensajes.Confirmar("¿Desactivar a " + seleccionado.NombreCompleto() + "?\nSu historial de notas y asistencia se conserva."))
                return;

            try
            {
                seleccionado.Desactivar();
                repositorio.Actualizar(seleccionado);
                Bitacora.Registrar(AccionAuditoria.EstudianteDesactivado, "Estudiante " + seleccionado.Rut + " desactivado.",
                    entidad: "Estudiante", entidadId: seleccionado.Id, valorAnterior: "Activo", valorNuevo: "Inactivo");
                Mensajes.Info("Estudiante desactivado.");
                CargarEstudiantes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}