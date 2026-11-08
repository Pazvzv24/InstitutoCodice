using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App.Formularios
{
    /// <summary>
    /// Informes por estudiante y por curso (RF-09, RF-10, RF-11).
    /// Control de acceso: el administrador ve todo; un docente solo ve
    /// los estudiantes y asignaturas de los cursos que dicta.
    /// Exportación a Excel y PDF de lo que está en pantalla (ya filtrado por permisos);
    /// cada exportación queda en la bitácora de auditoría.
    /// F5 o "Recargar" vuelve a leer las listas y el informe mostrado.
    /// </summary>
    public partial class FormInformes : Form
    {
        // Dependencias: llegan por el constructor (las entrega el Contenedor)
        private readonly EstudianteRepositorio estudianteRepositorio;
        private readonly AsignaturaRepositorio asignaturaRepositorio;
        private readonly ServicioInformes servicio;

        // Asignaturas que el usuario conectado tiene permitido ver
        private List<Asignatura> asignaturasPermitidas = new List<Asignatura>();

        // Último informe mostrado (lo que se exporta)
        private DatosInforme? informeActual;
        private string tipoActual = string.Empty;        // "Estudiante" o "Curso"
        private string referenciaActual = string.Empty;  // RUT o código de asignatura

        public FormInformes(EstudianteRepositorio estudianteRepositorio,
                            AsignaturaRepositorio asignaturaRepositorio,
                            ServicioInformes servicio)
        {
            InitializeComponent();

            this.estudianteRepositorio = estudianteRepositorio;
            this.asignaturaRepositorio = asignaturaRepositorio;
            this.servicio = servicio;

            HabilitarExportacion(false);
        }

        private void FormInformes_Load(object sender, EventArgs e)
        {
            CargarListas();
        }

        /// <summary>
        /// Carga asignaturas y estudiantes permitidos, manteniendo lo que estaba seleccionado.
        /// </summary>
        private void CargarListas()
        {
            int? asignaturaAnterior = (cmbAsignatura.SelectedItem as Asignatura)?.Id;
            int? estudianteAnterior = (cmbEstudiante.SelectedItem as Estudiante)?.Id;
            try
            {
                asignaturasPermitidas = asignaturaRepositorio.ListarParaUsuario(Sesion.UsuarioActual).ToList();
                cmbAsignatura.DataSource = asignaturasPermitidas;
                var asignatura = asignaturasPermitidas.FirstOrDefault(a => a.Id == asignaturaAnterior);
                if (asignatura != null)
                    cmbAsignatura.SelectedItem = asignatura;

                // Un docente solo ve a los estudiantes inscritos en sus asignaturas
                List<Estudiante> estudiantes = Sesion.EsAdministrador
                    ? estudianteRepositorio.Listar().Where(x => x.PuedeTratarse()).ToList() // Ley 21.719: sin bloqueados
                    : servicio.ObtenerEstudiantesDe(asignaturasPermitidas);
                cmbEstudiante.DataSource = estudiantes;
                var estudiante = estudiantes.FirstOrDefault(x => x.Id == estudianteAnterior);
                if (estudiante != null)
                    cmbEstudiante.SelectedItem = estudiante;
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnVerEstudiante_Click(object sender, EventArgs e)
        {
            var estudiante = cmbEstudiante.SelectedItem as Estudiante;
            if (estudiante == null)
            {
                Mensajes.Advertencia("Seleccione un estudiante.");
                return;
            }

            try
            {
                // El informe individual solo incluye asignaturas permitidas para el usuario
                var lineas = Sesion.EsAdministrador
                    ? servicio.InformePorEstudiante(estudiante)
                    : servicio.InformePorEstudiante(estudiante, asignaturasPermitidas);

                lblTituloInforme.Text = "Informe del estudiante: " + estudiante.NombreCompleto() + " - " + estudiante.Rut
                                      + " - Curso " + estudiante.Curso;
                lblResumen.Text = "Asignaturas: " + lineas.Count
                                + " | Aprobadas: " + lineas.Count(l => l.Estado == ServicioInformes.Aprobado)
                                + " | Reprobadas: " + lineas.Count(l => l.Estado == ServicioInformes.Reprobado)
                                + " | Parciales: " + lineas.Count(l => l.Estado == ServicioInformes.Parcial)
                                + " | Sin notas: " + lineas.Count(l => l.Estado == ServicioInformes.SinNotas);
                MostrarLineas(lineas, true);
                GuardarInformeActual(lineas, true, "Estudiante", estudiante.Rut);
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void btnVerCurso_Click(object sender, EventArgs e)
        {
            var asignatura = cmbAsignatura.SelectedItem as Asignatura;
            if (asignatura == null)
            {
                Mensajes.Advertencia("Seleccione una asignatura.");
                return;
            }

            try
            {
                var lineas = servicio.InformePorCurso(asignatura);
                lblTituloInforme.Text = "Informe del curso: " + asignatura + " - Docente: " + asignatura.NombreDocente;
                var conNotas = lineas.Where(l => l.CantidadNotas > 0).ToList();
                string promedioCurso = conNotas.Count > 0 ? conNotas.Average(l => l.Promedio).ToString("0.0") : "-";
                lblResumen.Text = "Estudiantes: " + lineas.Count
                                + " | Aprobados: " + lineas.Count(l => l.Estado == ServicioInformes.Aprobado)
                                + " | Reprobados: " + lineas.Count(l => l.Estado == ServicioInformes.Reprobado)
                                + " | Parciales: " + lineas.Count(l => l.Estado == ServicioInformes.Parcial)
                                + " | Promedio del curso: " + promedioCurso;
                MostrarLineas(lineas, false);
                GuardarInformeActual(lineas, false, "Curso", asignatura.Codigo);
            }
            catch (Exception ex)
            {
                Mensajes.Error(ex);
            }
        }

        private void MostrarLineas(List<LineaInforme> lineas, bool porEstudiante)
        {
            colRut.Visible = !porEstudiante;
            colEstudiante.Visible = !porEstudiante;
            colAsignatura.Visible = porEstudiante;

            dgvInforme.Rows.Clear();
            foreach (var linea in lineas)
            {
                string promedio = linea.CantidadNotas > 0 ? linea.Promedio.ToString("0.0") : "-";
                string asistencia = linea.TotalClases > 0 ? linea.PorcentajeAsistencia.ToString("0.0") + " %" : "-";
                int indice = dgvInforme.Rows.Add(linea.Rut, linea.NombreEstudiante, linea.Asignatura,
                                                 linea.Notas, promedio, asistencia, linea.Estado);

                var celdaEstado = dgvInforme.Rows[indice].Cells[colEstado.Index];
                if (linea.Estado == ServicioInformes.Aprobado)
                    celdaEstado.Style.ForeColor = Color.DarkGreen;
                else if (linea.Estado == ServicioInformes.Reprobado)
                    celdaEstado.Style.ForeColor = Color.Firebrick;
                else if (linea.Estado == ServicioInformes.Parcial)
                    celdaEstado.Style.ForeColor = Color.DarkOrange;
                else
                    celdaEstado.Style.ForeColor = Color.DimGray;
            }

            dgvInforme.ClearSelection();
            if (lineas.Count == 0)
                lblResumen.Text = porEstudiante ? "El estudiante no está inscrito en asignaturas." : "La asignatura no tiene estudiantes inscritos.";
        }

        // ------------------------------------------------------------ Exportación

        private void GuardarInformeActual(List<LineaInforme> lineas, bool porEstudiante, string tipo, string referencia)
        {
            informeActual = new DatosInforme
            {
                Titulo = lblTituloInforme.Text,
                Resumen = lblResumen.Text,
                PorEstudiante = porEstudiante,
                Lineas = lineas,
                EmitidoPor = Sesion.UsuarioActual.Nombre + " (" + Sesion.UsuarioActual.NombreUsuario + ")",
                FechaEmision = DateTime.Now
            };
            tipoActual = tipo;
            referenciaActual = referencia;
            HabilitarExportacion(lineas.Count > 0);
        }

        private void HabilitarExportacion(bool habilitar)
        {
            btnExportarExcel.Enabled = habilitar;
            btnExportarPdf.Enabled = habilitar;
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            Exportar("xlsx", "Libro de Excel (*.xlsx)|*.xlsx", "Excel", ExportadorInformes.ExportarExcel);
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            Exportar("pdf", "Documento PDF (*.pdf)|*.pdf", "PDF", ExportadorInformes.ExportarPdf);
        }

        private void Exportar(string extension, string filtro, string formato, Action<DatosInforme, string> exportar)
        {
            if (informeActual == null || informeActual.Lineas.Count == 0)
            {
                Mensajes.Advertencia("Primero genere un informe con datos.");
                return;
            }

            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Guardar informe como " + formato;
                dialogo.Filter = filtro;
                dialogo.FileName = FormatoInforme.NombreArchivo(tipoActual, referenciaActual, DateTime.Now, extension);
                dialogo.OverwritePrompt = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    informeActual.FechaEmision = DateTime.Now;
                    exportar(informeActual, dialogo.FileName);
                    Cursor = Cursors.Default;

                    // Datos personales salen del sistema: queda registro de quién, qué y en qué formato
                    Bitacora.Registrar(AccionAuditoria.InformeExportado,
                        informeActual.Titulo + " (" + informeActual.Lineas.Count + " filas) exportado a " + formato + ".",
                        entidad: "Informe", valorNuevo: formato);

                    if (Mensajes.Confirmar("Informe exportado correctamente.\n\n¿Desea abrirlo ahora?"))
                        Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    Mensajes.Error(ex);
                }
            }
        }

        // ------------------------------------------------------------ Recargar (F5)

        private void btnRecargar_Click(object sender, EventArgs e) => Recargar();

        private void FormInformes_KeyDown(object sender, KeyEventArgs e)
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

            // Si había un informe en pantalla, se vuelve a generar con los datos actuales,
            // solo si el mismo curso o estudiante sigue disponible en la lista
            if (tipoActual == "Curso" && (cmbAsignatura.SelectedItem as Asignatura)?.Codigo == referenciaActual)
                btnVerCurso_Click(this, EventArgs.Empty);
            else if (tipoActual == "Estudiante" && (cmbEstudiante.SelectedItem as Estudiante)?.Rut == referenciaActual)
                btnVerEstudiante_Click(this, EventArgs.Empty);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}