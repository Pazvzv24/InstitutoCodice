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
    /// - Evaluaciones: agregar, editar (sin superar el 100 %) y eliminar (solo si no tienen notas).
    /// - Notas: registrar, modificar y eliminar (dejando la celda vacía).
    /// Cada cambio queda en la bitácora con su valor anterior y nuevo.
    /// F5 o "Recargar" vuelve a leer los datos desde la base.
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

        // Notas que tenía cada estudiante al cargar la lista (para saber cuáles se eliminaron)
        private Dictionary<int, decimal> notasCargadas = new Dictionary<int, decimal>();

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
            CargarAsignaturas();
            CargarEvaluaciones();
        }

        /// <summary>
        /// Carga las asignaturas del usuario, manteniendo la que estaba seleccionada.
        /// </summary>
        private void CargarAsignaturas()
        {
            int? idAnterior = (cmbAsignatura.SelectedItem as Asignatura)?.Id;
            try
            {
                cargando = true;
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
            finally
            {
                cargando = false;
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormCalificaciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Recargar();
                e.Handled = true;
            }
        }

        private void Recargar()
        {
            if (dgvNotas.Rows.Count > 0 &&
                !Mensajes.Confirmar("Se volverán a leer los datos y se perderán las notas que no haya guardado.\n\n¿Continuar?"))
                return;

            CargarAsignaturas();
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

        /// <summary>
        /// Deja el formulario de evaluación listo para agregar una nueva.
        /// </summary>
        private void ModoNuevaEvaluacion()
        {
            lblNuevaEval.Text = "Nueva evaluación:";
            btnAgregarEval.Text = "Agregar evaluación";
            btnEliminarEval.Enabled = false;
            txtNombreEval.Clear();
            dtpFechaEval.Value = DateTime.Today;
            nudPonderacion.Value = 25;
        }

        /// <summary>
        /// Muestra la evaluación seleccionada en el formulario para editarla o eliminarla.
        /// </summary>
        private void ModoEditarEvaluacion(Evaluacion evaluacion)
        {
            lblNuevaEval.Text = "Editar evaluación seleccionada:";
            btnAgregarEval.Text = "Guardar cambios";
            btnEliminarEval.Enabled = true;
            txtNombreEval.Text = evaluacion.Nombre;
            dtpFechaEval.Value = evaluacion.Fecha;
            nudPonderacion.Value = Math.Min(Math.Max(evaluacion.Ponderacion, nudPonderacion.Minimum), nudPonderacion.Maximum);
        }

        private void btnNuevaEval_Click(object sender, EventArgs e)
        {
            LimpiarNotas();
            dgvEvaluaciones.ClearSelection();
        }

        private void LimpiarNotas()
        {
            evaluacionActual = null;
            notasCargadas = new Dictionary<int, decimal>();
            dgvNotas.Rows.Clear();
            lblEvaluacionSel.Text = "Seleccione una evaluación de la lista.";
            ModoNuevaEvaluacion();
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

            // Con una evaluación seleccionada, el botón guarda los cambios
            if (evaluacionActual != null)
            {
                GuardarCambiosEvaluacion(asignaturaActual, evaluacionActual, nombre);
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

        private void GuardarCambiosEvaluacion(Asignatura asignatura, Evaluacion evaluacion, string nombre)
        {
            DateTime fecha = dtpFechaEval.Value.Date;
            decimal ponderacion = nudPonderacion.Value;

            if (nombre == evaluacion.Nombre && fecha == evaluacion.Fecha.Date && ponderacion == evaluacion.Ponderacion)
            {
                Mensajes.Info("No hay cambios que guardar.");
                return;
            }

            string antes = DescribirEvaluacion(evaluacion.Nombre, evaluacion.Fecha, evaluacion.Ponderacion);
            try
            {
                asignatura.ActualizarEvaluacion(evaluacion.Id, nombre, fecha, ponderacion); // valida el 100 %
                evaluacionRepositorio.Actualizar(evaluacion);
                Bitacora.Registrar(AccionAuditoria.EvaluacionModificada,
                    "Evaluación (Id " + evaluacion.Id + ") de " + asignatura.Codigo + " modificada.",
                    entidad: "Evaluacion", entidadId: evaluacion.Id,
                    valorAnterior: antes, valorNuevo: DescribirEvaluacion(nombre, fecha, ponderacion));
                Mensajes.Info("Evaluación actualizada.");
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
            CargarEvaluaciones();
        }

        private void btnEliminarEval_Click(object sender, EventArgs e)
        {
            if (asignaturaActual == null || evaluacionActual == null)
            {
                Mensajes.Advertencia("Seleccione la evaluación que desea eliminar.");
                return;
            }

            var evaluacion = evaluacionActual;
            if (!Mensajes.Confirmar("¿Eliminar la evaluación \"" + evaluacion.Nombre + "\" (" + evaluacion.Ponderacion.ToString("0.##") + " %)?"))
                return;

            try
            {
                // La base solo la elimina si no tiene notas (además lo impide la clave foránea)
                if (!evaluacionRepositorio.EliminarSiNoTieneNotas(evaluacion.Id))
                {
                    Mensajes.Advertencia("No se puede eliminar \"" + evaluacion.Nombre + "\" porque ya tiene notas registradas.\n\n"
                        + "Si realmente debe eliminarse, primero borre sus notas (deje las celdas vacías y guarde).");
                    return;
                }

                asignaturaActual.QuitarEvaluacion(evaluacion.Id);
                Bitacora.Registrar(AccionAuditoria.EvaluacionEliminada,
                    "Evaluación (Id " + evaluacion.Id + ") de " + asignaturaActual.Codigo + " eliminada (no tenía notas).",
                    entidad: "Evaluacion", entidadId: evaluacion.Id,
                    valorAnterior: DescribirEvaluacion(evaluacion.Nombre, evaluacion.Fecha, evaluacion.Ponderacion));
                Mensajes.Info("Evaluación eliminada.");
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
            CargarEvaluaciones();
        }

        private static string DescribirEvaluacion(string nombre, DateTime fecha, decimal ponderacion)
        {
            return nombre + " | " + fecha.ToString("dd-MM-yyyy") + " | " + ponderacion.ToString("0.##") + " %";
        }

        private void dgvEvaluaciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || asignaturaActual == null) return;

            int id = Convert.ToInt32(dgvEvaluaciones.Rows[e.RowIndex].Cells[colEvalId.Index].Value);
            evaluacionActual = asignaturaActual.Evaluaciones.FirstOrDefault(x => x.Id == id);
            CargarNotas();
            if (evaluacionActual != null)
                ModoEditarEvaluacion(evaluacionActual);
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
                notasCargadas = new Dictionary<int, decimal>();
                foreach (var c in notas)
                    notasCargadas[c.Estudiante.Id] = c.Nota;

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
            var eliminar = new List<int>(); // estudiantes cuya nota quedó vacía

            foreach (DataGridViewRow fila in dgvNotas.Rows)
            {
                int estudianteId = Convert.ToInt32(fila.Cells[colEstId.Index].Value);
                string texto = (Convert.ToString(fila.Cells[colNota.Index].Value) ?? "").Trim();
                if (texto.Length == 0)
                {
                    if (notasCargadas.ContainsKey(estudianteId))
                        eliminar.Add(estudianteId); // tenía nota y ahora está vacía: se elimina
                    continue;
                }

                string nombre = Convert.ToString(fila.Cells[colNotaNombre.Index].Value) ?? "";
                if (!IntentarLeerNota(texto, out decimal valor))
                {
                    Mensajes.Advertencia("La nota de " + nombre + " (\"" + texto + "\") no es un número válido.");
                    return;
                }

                var calificacion = new Calificacion
                {
                    Estudiante = new Estudiante { Id = estudianteId },
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

            if (lista.Count == 0 && eliminar.Count == 0)
            {
                Mensajes.Advertencia("No hay notas para guardar.");
                return;
            }

            if (eliminar.Count > 0 &&
                !Mensajes.Confirmar("Se eliminarán " + eliminar.Count + " nota(s) que quedaron vacías.\n"
                    + "Cada eliminación queda registrada en la bitácora.\n\n¿Continuar?"))
                return;

            try
            {
                // Guarda y elimina en una sola transacción, con auditoría de cada nota
                calificacionRepositorio.GuardarCambios(lista, evaluacionActual, eliminar, Sesion.UsuarioActual);
                Mensajes.Info("Se guardaron " + lista.Count + " nota(s)"
                    + (eliminar.Count > 0 ? " y se eliminaron " + eliminar.Count : "") + ".");
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