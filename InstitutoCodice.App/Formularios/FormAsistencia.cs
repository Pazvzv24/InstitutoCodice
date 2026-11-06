using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Gestión de asistencia por fecha (RF-05, RF-06).
    /// </summary>
    public partial class FormAsistencia : Form
    {
        private readonly AsignaturaRepositorio asignaturaRepositorio = new AsignaturaRepositorio();
        private readonly AsistenciaRepositorio asistenciaRepositorio = new AsistenciaRepositorio();
        private readonly InformeRepositorio informeRepositorio = new InformeRepositorio();

        private Asignatura asignaturaCargada;
        private DateTime fechaCargada;

        public FormAsistencia()
        {
            InitializeComponent();
        }

        private void FormAsistencia_Load(object sender, EventArgs e)
        {
            dtpFecha.MaxDate = DateTime.Today;
            dtpFecha.Value = DateTime.Today;

            try
            {
                cmbAsignatura.DataSource = asignaturaRepositorio.ListarParaUsuario(Sesion.UsuarioActual);
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            CargarLista();
        }

        private void CargarLista()
        {
            var asignatura = cmbAsignatura.SelectedItem as Asignatura;
            if (asignatura == null)
            {
                Mensajes.Advertencia("Seleccione una asignatura.");
                return;
            }

            var fecha = dtpFecha.Value.Date;
            try
            {
                var inscritos = informeRepositorio.ObtenerInscritos(asignatura.Id).Where(x => x.Activo).ToList();
                var registradas = asistenciaRepositorio.ListarPorAsignaturaYFecha(asignatura.Id, fecha);

                dgvAsistencia.Rows.Clear();
                foreach (var estudiante in inscritos)
                {
                    var existente = registradas.FirstOrDefault(a => a.Estudiante.Id == estudiante.Id);
                    var estado = existente != null ? existente.Estado : EstadoAsistencia.Presente;
                    dgvAsistencia.Rows.Add(estudiante.Id, estudiante.Rut, estudiante.NombreCompleto(), estado.ToString());
                }

                asignaturaCargada = asignatura;
                fechaCargada = fecha;

                if (inscritos.Count == 0)
                    lblEstadoCarga.Text = "No hay estudiantes activos inscritos en esta asignatura.";
                else if (registradas.Count > 0)
                    lblEstadoCarga.Text = "Ya hay asistencia registrada el " + fecha.ToString("dd-MM-yyyy") + ". Al guardar se modificará.";
                else
                    lblEstadoCarga.Text = "Nueva toma de asistencia para el " + fecha.ToString("dd-MM-yyyy") + ".";
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnTodosPresentes_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow fila in dgvAsistencia.Rows)
                fila.Cells[colEstado.Index].Value = EstadoAsistencia.Presente.ToString();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (asignaturaCargada == null || dgvAsistencia.Rows.Count == 0)
            {
                Mensajes.Advertencia("Primero cargue la lista de estudiantes.");
                return;
            }

            dgvAsistencia.EndEdit();
            var lista = new List<Asistencia>();

            foreach (DataGridViewRow fila in dgvAsistencia.Rows)
            {
                var asistencia = new Asistencia
                {
                    Estudiante = new Estudiante { Id = Convert.ToInt32(fila.Cells[colEstudianteId.Index].Value) },
                    Asignatura = asignaturaCargada,
                    Fecha = fechaCargada
                };

                string estadoTexto = Convert.ToString(fila.Cells[colEstado.Index].Value);
                asistencia.CambiarEstado((EstadoAsistencia)Enum.Parse(typeof(EstadoAsistencia), estadoTexto));

                if (!asistencia.ValidarFecha())
                {
                    Mensajes.Advertencia("No se puede registrar asistencia en una fecha futura.");
                    return;
                }

                lista.Add(asistencia);
            }

            try
            {
                asistenciaRepositorio.GuardarLista(lista);
                int presentes = lista.Count(a => a.Estado == EstadoAsistencia.Presente);
                Mensajes.Info("Asistencia guardada: " + presentes + " de " + lista.Count + " presentes.");
                CargarLista();
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