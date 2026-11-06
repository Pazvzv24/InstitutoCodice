using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Evaluaciones y calificaciones (RF-07, RF-08).
    /// Cada nota registrada o modificada queda en la bitácora con su valor anterior y nuevo.
    /// </summary>
    public partial class FormCalificaciones : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly AsignaturaRepositorio asignaturaRepositorio;
        private readonly EvaluacionRepositorio evaluacionRepositorio;
        private readonly CalificacionRepositorio calificacionRepositorio;
        private readonly InformeRepositorio informeRepositorio;

        // Selección actual en pantalla: es null hasta que el usuario elige algo
        private Asignatura? asignaturaActual;
        private Evaluacion? evaluacionActual;
        private bool cargando;

        public FormCalificaciones(AsignaturaRepositorio asignaturaRepositorio,
                                  EvaluacionRepositorio evaluacionRepositorio,
                                  CalificacionRepositorio calificacionRepositorio,
                                  InformeRepositorio informeRepositorio)
        {
            InitializeComponent();
            this.asignaturaRepositorio = asignaturaRepositorio;
            this.evaluacionRepositorio = evaluacionRepositorio;
            this.calificacionRepositorio = calificacionRepositorio;
            this.informeRepositorio = informeRepositorio;
        }

        private void FormCalificaciones_Load(object sender, EventArgs e)
        {
            try
            {
                cargando = true;
                cmbAsignatura.DataSource = asignaturaRepositorio.ListarParaUsuario(Sesion.UsuarioActual);
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
            finally
            {
                cargando = false;
            }

            CargarEvaluaciones();
        }

        private void cmbAsignatura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cargando)
                CargarEvaluaciones();
        }

        private void CargarEvaluaciones()
        {
            dgvEvaluaciones.Rows.Clear();
            LimpiarNotas();

            asignaturaActual = cmbAsignatura.SelectedItem as Asignatura;
            if (asignaturaActual == null) return;

            try
            {
                asignaturaActual.Evaluaciones = evaluacionRepositorio.ListarPorAsignatura(asignaturaActual.Id);
                foreach (var evaluacion in asignaturaActual.Evaluaciones)
                {
                    dgvEvaluaciones.Rows.Add(evaluacion.Id, evaluacion.Nombre,
                        evaluacion.Fecha.ToString("dd-MM-yyyy"), evaluacion.Ponderacion.ToString("0.##"));
                }

                decimal suma = asignaturaActual.SumaPonderaciones();
                lblSuma.Text = "Suma de ponderaciones: " + suma.ToString("0.##") + " %   (disponible: "
                             + (Asignatura.PonderacionMaxima - suma).ToString("0.##") + " %)";
                dgvEvaluaciones.ClearSelection();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void LimpiarNotas()
        {
            evaluacionActual = null;
            dgvNotas.Rows.Clear();
            lblEvaluacionSel.Text = "Seleccione una evaluación de la lista.";
        }

        private void btnAgregarEval_Click(object sender, EventArgs e)
        {
            if (asignaturaActual == null)
            {
                Mensajes.Advertencia("Seleccione una asignatura.");
                return;
            }

            string nombre = txtNombreEval.Text.Trim();
            if (nombre.Length == 0)
            {
                Mensajes.Advertencia("Ingrese el nombre de la evaluación.");
                return;
            }

            var evaluacion = new Evaluacion
            {
                Nombre = nombre,
                Fecha = dtpFechaEval.Value.Date,
                Ponderacion = nudPonderacion.Value
            };

            try
            {
                asignaturaActual.AgregarEvaluacion(evaluacion);
                evaluacionRepositorio.Agregar(evaluacion, asignaturaActual.Id);
                Bitacora.Registrar(AccionAuditoria.EvaluacionCreada,
                    "Evaluación \"" + evaluacion.Nombre + "\" (" + evaluacion.Ponderacion.ToString("0.##") + " %) en " + asignaturaActual.Codigo + ".",
                    entidad: "Evaluacion", entidadId: evaluacion.Id);
                txtNombreEval.Clear();
                CargarEvaluaciones();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
                CargarEvaluaciones();
            }
        }

        private void dgvEvaluaciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || asignaturaActual == null) return;

            int id = Convert.ToInt32(dgvEvaluaciones.Rows[e.RowIndex].Cells[colEvalId.Index].Value);
            evaluacionActual = asignaturaActual.Evaluaciones.FirstOrDefault(x => x.Id == id);
            CargarNotas();
        }

        private void CargarNotas()
        {
            dgvNotas.Rows.Clear();
            if (evaluacionActual == null || asignaturaActual == null) return;

            lblEvaluacionSel.Text = evaluacionActual.Nombre + " - " + evaluacionActual.Ponderacion.ToString("0.##")
                                  + " % - " + evaluacionActual.Fecha.ToString("dd-MM-yyyy");
            try
            {
                var inscritos = informeRepositorio.ObtenerInscritos(asignaturaActual.Id).Where(x => x.PuedeTratarse()); // Ley 21.719: sin bloqueados
                var notas = calificacionRepositorio.ListarPorEvaluacion(evaluacionActual.Id);

                foreach (var estudiante in inscritos)
                {
                    var existente = notas.FirstOrDefault(c => c.Estudiante.Id == estudiante.Id);
                    string nota = existente != null ? existente.Nota.ToString("0.0") : "";
                    dgvNotas.Rows.Add(estudiante.Id, estudiante.Rut, estudiante.NombreCompleto(), nota);
                }
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnGuardarNotas_Click(object sender, EventArgs e)
        {
            if (evaluacionActual == null)
            {
                Mensajes.Advertencia("Seleccione una evaluación.");
                return;
            }

            dgvNotas.EndEdit();
            var lista = new List<Calificacion>();

            foreach (DataGridViewRow fila in dgvNotas.Rows)
            {
                string texto = (Convert.ToString(fila.Cells[colNota.Index].Value) ?? "").Trim();
                if (texto.Length == 0) continue;

                string nombre = Convert.ToString(fila.Cells[colNotaNombre.Index].Value) ?? "";
                if (!IntentarLeerNota(texto, out decimal valor))
                {
                    Mensajes.Advertencia("La nota de " + nombre + " (\"" + texto + "\") no es un número válido.");
                    return;
                }

                var calificacion = new Calificacion
                {
                    Estudiante = new Estudiante { Id = Convert.ToInt32(fila.Cells[colEstId.Index].Value) },
                    Evaluacion = evaluacionActual,
                    Nota = valor
                };

                if (!calificacion.ValidarNota())
                {
                    Mensajes.Advertencia("La nota de " + nombre + " (" + texto + ") debe estar entre 1,0 y 7,0.");
                    dgvNotas.CurrentCell = fila.Cells[colNota.Index];
                    return;
                }

                lista.Add(calificacion);
            }

            if (lista.Count == 0)
            {
                Mensajes.Advertencia("No hay notas para guardar.");
                return;
            }

            try
            {
                calificacionRepositorio.GuardarLista(lista, Sesion.UsuarioActual); // incluye auditoría de cada nota
                Mensajes.Info("Se guardaron " + lista.Count + " notas.");
                CargarNotas();
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private static bool IntentarLeerNota(string texto, out decimal nota)
        {
            bool ok = decimal.TryParse(texto.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out nota);
            if (ok)
                nota = Math.Round(nota, 1, MidpointRounding.AwayFromZero);
            return ok;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}