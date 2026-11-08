using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Inscripción de estudiantes en asignaturas (RF-04).
    /// F5 o "Recargar" vuelve a leer asignaturas, estudiantes e inscritos desde la base.
    /// </summary>
    public partial class FormInscripciones : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly InscripcionRepositorio repositorio;
        private readonly AsignaturaRepositorio asignaturaRepositorio;
        private readonly EstudianteRepositorio estudianteRepositorio;

        private List<Inscripcion> inscripciones = new List<Inscripcion>();
        private bool cargando;

        public FormInscripciones(InscripcionRepositorio repositorio,
                                 AsignaturaRepositorio asignaturaRepositorio,
                                 EstudianteRepositorio estudianteRepositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            this.asignaturaRepositorio = asignaturaRepositorio;
            this.estudianteRepositorio = estudianteRepositorio;
        }

        private void FormInscripciones_Load(object sender, EventArgs e)
        {
            CargarListas();
            CargarInscritos();
        }

        /// <summary>
        /// Carga asignaturas y estudiantes, manteniendo lo que estaba seleccionado.
        /// </summary>
        private void CargarListas()
        {
            int? asignaturaAnterior = (cmbAsignatura.SelectedItem as Asignatura)?.Id;
            int? estudianteAnterior = (cmbEstudiante.SelectedItem as Estudiante)?.Id;
            try
            {
                cargando = true;
                var asignaturas = asignaturaRepositorio.Listar().ToList();
                cmbAsignatura.DataSource = asignaturas;
                var asignatura = asignaturas.FirstOrDefault(a => a.Id == asignaturaAnterior);
                if (asignatura != null)
                    cmbAsignatura.SelectedItem = asignatura;

                var estudiantes = estudianteRepositorio.Listar().Where(x => x.Activo && x.PuedeTratarse()).ToList(); // Ley 21.719: sin bloqueados
                cmbEstudiante.DataSource = estudiantes;
                var estudiante = estudiantes.FirstOrDefault(x => x.Id == estudianteAnterior);
                if (estudiante != null)
                    cmbEstudiante.SelectedItem = estudiante;
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
            finally
            {
                cargando = false;
            }
        }

        private void cmbAsignatura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cargando)
                CargarInscritos();
        }

        private void CargarInscritos()
        {
            dgvInscritos.Rows.Clear();
            var asignatura = cmbAsignatura.SelectedItem as Asignatura;
            if (asignatura == null)
            {
                lblTotal.Text = "Total inscritos: 0";
                return;
            }

            try
            {
                inscripciones = repositorio.ListarPorAsignatura(asignatura.Id);
                foreach (var inscripcion in inscripciones)
                {
                    var estudiante = inscripcion.Estudiante;
                    dgvInscritos.Rows.Add(inscripcion.Id, estudiante.Rut,
                        estudiante.NombreCompleto() + (estudiante.Activo ? "" : " (inactivo)"),
                        estudiante.Curso, inscripcion.FechaInscripcion.ToString("dd-MM-yyyy"));
                }

                lblTotal.Text = "Total inscritos: " + inscripciones.Count;
                dgvInscritos.ClearSelection();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnInscribir_Click(object sender, EventArgs e)
        {
            var asignatura = cmbAsignatura.SelectedItem as Asignatura;
            var estudiante = cmbEstudiante.SelectedItem as Estudiante;
            if (asignatura == null || estudiante == null)
            {
                Mensajes.Advertencia("Seleccione una asignatura y un estudiante.");
                return;
            }

            var nueva = new Inscripcion
            {
                Estudiante = estudiante,
                Asignatura = asignatura,
                FechaInscripcion = DateTime.Today
            };

            if (nueva.EsDuplicada(inscripciones))
            {
                Mensajes.Advertencia(estudiante.NombreCompleto() + " ya está inscrito(a) en " + asignatura.Nombre + ".");
                return;
            }

            try
            {
                repositorio.Agregar(nueva, Sesion.UsuarioActual); // se audita en la misma transacción
                CargarInscritos();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvInscritos.CurrentRow == null || dgvInscritos.SelectedRows.Count == 0)
            {
                Mensajes.Advertencia("Seleccione una inscripción de la lista.");
                return;
            }

            int id = Convert.ToInt32(dgvInscritos.CurrentRow.Cells[colId.Index].Value);
            string nombre = Convert.ToString(dgvInscritos.CurrentRow.Cells[colEstudiante.Index].Value) ?? "";
            if (!Mensajes.Confirmar("¿Quitar la inscripción de " + nombre + "?\nSus notas y asistencia registradas se conservan."))
                return;

            try
            {
                repositorio.Eliminar(id, Sesion.UsuarioActual); // se audita en la misma transacción
                CargarInscritos();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormInscripciones_KeyDown(object sender, KeyEventArgs e)
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
            CargarListas();
            CargarInscritos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}