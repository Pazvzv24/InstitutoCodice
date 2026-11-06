using System;
using System.Collections.Generic;
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
    /// </summary>
    public partial class FormInformes : Form
    {
        private readonly EstudianteRepositorio estudianteRepositorio = new EstudianteRepositorio();
        private readonly AsignaturaRepositorio asignaturaRepositorio = new AsignaturaRepositorio();
        private readonly ServicioInformes servicio = new ServicioInformes(new InformeRepositorio());

        // Asignaturas que el usuario conectado tiene permitido ver
        private List<Asignatura> asignaturasPermitidas = new List<Asignatura>();

        public FormInformes()
        {
            InitializeComponent();
        }

        private void FormInformes_Load(object sender, EventArgs e)
        {
            try
            {
                asignaturasPermitidas = asignaturaRepositorio.ListarParaUsuario(Sesion.UsuarioActual).ToList();
                cmbAsignatura.DataSource = asignaturasPermitidas;

                // Un docente solo ve a los estudiantes inscritos en sus asignaturas
                if (Sesion.EsAdministrador)
                    cmbEstudiante.DataSource = estudianteRepositorio.Listar();
                else
                    cmbEstudiante.DataSource = servicio.ObtenerEstudiantesDe(asignaturasPermitidas);
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}