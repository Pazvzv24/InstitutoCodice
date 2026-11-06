using System;
using System.Collections.Generic;
using System.Linq;
using InstitutoCodice.Negocio.Interfaces;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Servicio encargado del cálculo académico y la generación de informes.
    /// </summary>
    public class ServicioInformes
    {
        public const decimal NotaAprobacion = 4.0m;
        public const decimal AsistenciaMinima = 75m;
        public const decimal PonderacionCompleta = 100m;
        public const string Aprobado = "Aprobado";
        public const string Reprobado = "Reprobado";
        public const string SinNotas = "Sin notas";
        public const string Parcial = "Parcial";

        private readonly IFuenteDatosInformes fuente;

        public ServicioInformes(IFuenteDatosInformes fuente)
        {
            if (fuente == null)
                throw new ArgumentNullException("fuente");

            this.fuente = fuente;
        }

        public decimal CalcularPromedio(Estudiante estudiante, Asignatura asignatura)
        {
            return CalcularPromedioPonderado(fuente.ObtenerCalificaciones(estudiante.Id, asignatura.Id));
        }

        public decimal CalcularPorcentajeAsistencia(Estudiante estudiante, Asignatura asignatura)
        {
            return CalcularPorcentaje(fuente.ObtenerAsistencias(estudiante.Id, asignatura.Id));
        }

        /// <summary>
        /// Calcula el promedio ponderado considerando solo evaluaciones con ponderación.
        /// </summary>
        public static decimal CalcularPromedioPonderado(IEnumerable<Calificacion> calificaciones)
        {
            List<Calificacion> lista = (calificaciones ?? Enumerable.Empty<Calificacion>())
                .Where(c => c.Evaluacion != null && c.Evaluacion.Ponderacion > 0)
                .ToList();

            decimal sumaPonderaciones = lista.Sum(c => c.Evaluacion.Ponderacion);
            if (sumaPonderaciones == 0)
                return 0m;

            decimal sumaPonderada = lista.Sum(c => c.Nota * c.Evaluacion.Ponderacion);
            return Math.Round(sumaPonderada / sumaPonderaciones, 1, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Determina el porcentaje de asistencia sobre el total de registros.
        /// </summary>
        public static decimal CalcularPorcentaje(IEnumerable<Asistencia> asistencias)
        {
            List<Asistencia> lista = (asistencias ?? Enumerable.Empty<Asistencia>()).ToList();
            if (lista.Count == 0)
                return 0m;

            decimal presentes = lista.Count(a => a.Estado == EstadoAsistencia.Presente);
            return Math.Round(presentes / lista.Count * 100m, 1, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Indica si el resultado es definitivo: el estudiante tiene nota en evaluaciones
        /// que suman el 100% de la ponderación. Si suman menos, el resultado es parcial.
        /// (Las ponderaciones de una asignatura no superan el 100%.)
        /// </summary>
        public static bool EsDefinitivo(IEnumerable<Calificacion> calificaciones)
        {
            decimal sumaPonderaciones = (calificaciones ?? Enumerable.Empty<Calificacion>())
                .Where(c => c.Evaluacion != null)
                .Sum(c => c.Evaluacion.Ponderacion);

            return sumaPonderaciones >= PonderacionCompleta;
        }

        /// <summary>
        /// Define si el estudiante aprueba según nota y asistencia mínimas.
        /// </summary>
        public string ObtenerEstado(decimal promedio, decimal asistencia)
        {
            return promedio >= NotaAprobacion && asistencia >= AsistenciaMinima ? Aprobado : Reprobado;
        }

        public List<LineaInforme> InformePorEstudiante(Estudiante estudiante)
        {
            List<LineaInforme> lineas = new List<LineaInforme>();
            foreach (Asignatura asignatura in fuente.ObtenerAsignaturasDeEstudiante(estudiante.Id))
                lineas.Add(CrearLinea(estudiante, asignatura));

            return lineas;
        }

        /// <summary>
        /// Informe individual restringido: incluye solo las asignaturas que el usuario
        /// tiene permitido ver (un docente, solo las que dicta).
        /// </summary>
        public List<LineaInforme> InformePorEstudiante(Estudiante estudiante, IEnumerable<Asignatura> asignaturasPermitidas)
        {
            HashSet<int> idsPermitidos = new HashSet<int>(
                (asignaturasPermitidas ?? Enumerable.Empty<Asignatura>()).Select(a => a.Id));

            List<LineaInforme> lineas = new List<LineaInforme>();
            foreach (Asignatura asignatura in fuente.ObtenerAsignaturasDeEstudiante(estudiante.Id))
            {
                if (idsPermitidos.Contains(asignatura.Id))
                    lineas.Add(CrearLinea(estudiante, asignatura));
            }

            return lineas;
        }

        /// <summary>
        /// Devuelve los estudiantes inscritos en al menos una de las asignaturas indicadas,
        /// sin repetidos y ordenados por nombre.
        /// </summary>
        public List<Estudiante> ObtenerEstudiantesDe(IEnumerable<Asignatura> asignaturas)
        {
            Dictionary<int, Estudiante> estudiantes = new Dictionary<int, Estudiante>();
            foreach (Asignatura asignatura in asignaturas ?? Enumerable.Empty<Asignatura>())
            {
                foreach (Estudiante estudiante in fuente.ObtenerInscritos(asignatura.Id))
                {
                    if (!estudiantes.ContainsKey(estudiante.Id))
                        estudiantes.Add(estudiante.Id, estudiante);
                }
            }

            return estudiantes.Values.OrderBy(e => e.NombreCompleto()).ToList();
        }

        public List<LineaInforme> InformePorCurso(Asignatura asignatura)
        {
            List<LineaInforme> lineas = new List<LineaInforme>();
            foreach (Estudiante estudiante in fuente.ObtenerInscritos(asignatura.Id))
                lineas.Add(CrearLinea(estudiante, asignatura));

            return lineas;
        }

        private LineaInforme CrearLinea(Estudiante estudiante, Asignatura asignatura)
        {
            List<Calificacion> calificaciones = fuente.ObtenerCalificaciones(estudiante.Id, asignatura.Id);
            List<Asistencia> asistencias = fuente.ObtenerAsistencias(estudiante.Id, asignatura.Id);

            decimal promedio = CalcularPromedioPonderado(calificaciones);
            decimal porcentaje = CalcularPorcentaje(asistencias);

            LineaInforme linea = new LineaInforme();
            linea.Rut = estudiante.Rut;
            linea.NombreEstudiante = estudiante.NombreCompleto();
            linea.Asignatura = asignatura.Nombre;
            linea.Notas = string.Join(" | ", calificaciones.Select(c =>
                c.Evaluacion.Nombre + ": " + c.Nota.ToString("0.0")));
            linea.CantidadNotas = calificaciones.Count;
            linea.Promedio = promedio;
            linea.TotalClases = asistencias.Count;
            linea.PorcentajeAsistencia = porcentaje;

            if (calificaciones.Count == 0)
                linea.Estado = SinNotas;
            else if (!EsDefinitivo(calificaciones))
                linea.Estado = Parcial;
            else
                linea.Estado = ObtenerEstado(promedio, porcentaje);

            return linea;
        }
    }
}