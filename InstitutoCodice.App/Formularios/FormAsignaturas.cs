using System;
using System.Text;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Gestión de asignaturas (RF-03).
    /// </summary>
    public partial class FormAsignaturas : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly AsignaturaRepositorio repositorio;
        private readonly DocenteRepositorio docenteRepositorio;

        private Asignatura seleccionada;

        public FormAsignaturas(AsignaturaRepositorio repositorio, DocenteRepositorio docenteRepositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            this.docenteRepositorio = docenteRepositorio;
            dgvAsignaturas.AutoGenerateColumns = false;
        }

        private void FormAsignaturas_Load(object sender, EventArgs e)
        {
            try
            {
                cmbDocente.DataSource = docenteRepositorio.Listar();
                cmbDocente.DisplayMember = "Nombre";
                cmbDocente.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }

            CargarAsignaturas();
            LimpiarFormulario();
        }

        private void CargarAsignaturas()
        {
            try
            {
                dgvAsignaturas.DataSource = null;
                dgvAsignaturas.DataSource = repositorio.Listar();
                dgvAsignaturas.ClearSelection();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void dgvAsignaturas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var asignatura = dgvAsignaturas.Rows[e.RowIndex].DataBoundItem as Asignatura;
            if (asignatura == null) return;

            seleccionada = asignatura;
            txtCodigo.Text = asignatura.Codigo;
            txtNombre.Text = asignatura.Nombre;
            txtCurso.Text = asignatura.Curso;
            cmbDocente.SelectedValue = asignatura.Docente.Id;
            lblModo.Text = "Editando: " + asignatura.Nombre;
        }

        private void LimpiarFormulario()
        {
            seleccionada = null;
            txtCodigo.Clear();
            txtNombre.Clear();
            txtCurso.Clear();
            cmbDocente.SelectedIndex = -1;
            lblModo.Text = "Modo: nueva asignatura";
            dgvAsignaturas.ClearSelection();
            txtCodigo.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var asignatura = new Asignatura
            {
                Id = seleccionada != null ? seleccionada.Id : 0,
                Codigo = txtCodigo.Text.Trim().ToUpper(),
                Nombre = txtNombre.Text.Trim(),
                Curso = txtCurso.Text.Trim(),
                Docente = cmbDocente.SelectedItem as Docente
            };

            var errores = new StringBuilder();
            if (asignatura.Codigo.Length == 0)
                errores.AppendLine("- El código es obligatorio.");
            if (asignatura.Nombre.Length == 0)
                errores.AppendLine("- El nombre es obligatorio.");
            if (asignatura.Curso.Length == 0)
                errores.AppendLine("- El curso es obligatorio.");
            if (asignatura.Docente == null)
                errores.AppendLine("- Seleccione el docente a cargo.");

            if (errores.Length > 0)
            {
                Mensajes.Advertencia("Revise los datos:\n" + errores);
                return;
            }

            try
            {
                if (asignatura.Id == 0)
                {
                    repositorio.Agregar(asignatura);
                    Mensajes.Info("Asignatura creada correctamente.");
                }
                else
                {
                    repositorio.Actualizar(asignatura);
                    Mensajes.Info("Asignatura actualizada.");
                }

                CargarAsignaturas();
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