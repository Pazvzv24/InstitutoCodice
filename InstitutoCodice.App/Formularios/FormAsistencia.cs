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
    /// F5 o "Recargar" vuelve a leer las asignaturas y la lista cargada desde la base.
    /// Cada asistencia registrada o modificada queda en la bitácora.
    /// </summary>
    public partial class FormAsistencia : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly AsignaturaRepositorio asignaturaRepositorio;
        private readonly AsistenciaRepositorio asistenciaRepositorio;
        private readonly InformeRepositorio informeRepositorio;

        // Lista cargada en pantalla: es null hasta que se presiona "Cargar"
        private Asignatura? asignaturaCargada;
        private DateTime fechaCargada;

        public FormAsistencia(AsignaturaRepositorio asignaturaRepositorio,
                              AsistenciaRepositorio asistenciaRepositorio,
                              InformeRepositorio informeRepositorio)
        {
            InitializeComponent();
            this.asignaturaRepositorio = asignaturaRepositorio;
            this.asistenciaRepositorio = asistenciaRepositorio;
            this.informeRepositorio = informeRepositorio;
        }

        private void FormAsistencia_Load(object sender, EventArgs e)
        {
            dtpFecha.MaxDate = DateTime.Today;
            dtpFecha.Value = DateTime.Today;

            CargarAsignaturas();
        }

        /// <summary>
        /// Carga las asignaturas del usuario, manteniendo la que estaba seleccionada.
        /// </summary>
        private void CargarAsignaturas()
        {
            int? idAnterior = (cmbAsignatura.SelectedItem as Asignatura)?.Id;
            try
            {
                var asignaturas = asignaturaRepositorio.ListarParaUsuario(Sesion.UsuarioActual).ToList();
                cmbAsignatura.DataSource = asignaturas;
                var previa = asignaturas.FirstOrDefault(a => a.Id == idAnterior);
                if (previa != null)
                    cmbAsignatura.SelectedItem = previa;
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
                var inscritos = informeRepositorio.ObtenerInscritos(asignatura.Id).Where(x => x.Activo && x.PuedeTratarse()).ToList(); // Ley 21.719: sin bloqueados
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

                string estadoTexto = Convert.ToString(fila.Cells[colEstado.Index].Value) ?? "";
                if (!Enum.TryParse(estadoTexto, out EstadoAsistencia estado))
                {
                    Mensajes.Advertencia("Hay un estado de asistencia no válido (\"" + estadoTexto + "\"). Revise la lista.");
                    return;
                }
                asistencia.CambiarEstado(estado);

                if (!asistencia.ValidarFecha())
                {
                    Mensajes.Advertencia("No se puede registrar asistencia en una fecha futura.");
                    return;
                }

                lista.Add(asistencia);
            }

            try
            {
                // Guarda y audita en la misma transacción (solo los estados que cambiaron)
                asistenciaRepositorio.GuardarLista(lista, Sesion.UsuarioActual);
                int presentes = lista.Count(a => a.Estado == EstadoAsistencia.Presente);
                Mensajes.Info("Asistencia guardada: " + presentes + " de " + lista.Count + " presentes.");
                CargarLista();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormAsistencia_KeyDown(object sender, KeyEventArgs e)
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
            if (dgvAsistencia.Rows.Count > 0 &&
                !Mensajes.Confirmar("Se volverá a leer la lista y se perderán los cambios de asistencia que no haya guardado.\n\n¿Continuar?"))
                return;

            CargarAsignaturas();
            if (asignaturaCargada != null)
                CargarLista();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}