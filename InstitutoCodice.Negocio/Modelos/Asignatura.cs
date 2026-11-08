using System;
using System.Collections.Generic;
using System.Linq;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa una asignatura del plan de estudios y su conjunto de evaluaciones.
    /// </summary>
    public class Asignatura
    {
        public const decimal PonderacionMaxima = 100m;

        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public Docente Docente { get; set; } = new Docente();
        public List<Evaluacion> Evaluaciones { get; set; }

        public Asignatura()
        {
            Evaluaciones = new List<Evaluacion>();
        }

        /// <summary>
        /// Nombre del docente asignado, utilizado en listados.
        /// </summary>
        public string NombreDocente
        {
            get { return Docente != null ? Docente.Nombre : ""; }
        }

        /// <summary>
        /// Incorpora una evaluación verificando que no se exceda la ponderación total permitida.
        /// </summary>
        public void AgregarEvaluacion(Evaluacion evaluacion)
        {
            if (evaluacion == null)
                throw new ArgumentNullException("evaluacion");

            if (!evaluacion.ValidarPonderacion())
                throw new ArgumentException("La ponderación debe ser mayor que 0 y como máximo 100 %.");

            decimal disponible = PonderacionMaxima - SumaPonderaciones();
            if (evaluacion.Ponderacion > disponible)
                throw new InvalidOperationException(
                    "La suma de ponderaciones no puede superar 100 %. Disponible: " + disponible.ToString("0.##") + " %.");

            Evaluaciones.Add(evaluacion);
        }

        /// <summary>
        /// Modifica una evaluación existente validando que la suma de ponderaciones
        /// siga sin superar el 100 % (se descuenta la ponderación que tenía antes).
        /// Si la validación falla, la evaluación queda sin cambios.
        /// </summary>
        public void ActualizarEvaluacion(int evaluacionId, string nombre, DateTime fecha, decimal ponderacion)
        {
            Evaluacion? evaluacion = Evaluaciones.FirstOrDefault(e => e.Id == evaluacionId);
            if (evaluacion == null)
                throw new InvalidOperationException("La evaluación no pertenece a esta asignatura.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la evaluación es obligatorio.");

            var propuesta = new Evaluacion { Ponderacion = ponderacion };
            if (!propuesta.ValidarPonderacion())
                throw new ArgumentException("La ponderación debe ser mayor que 0 y como máximo 100 %.");

            decimal disponible = PonderacionMaxima - (SumaPonderaciones() - evaluacion.Ponderacion);
            if (ponderacion > disponible)
                throw new InvalidOperationException(
                    "La suma de ponderaciones no puede superar 100 %. Disponible para esta evaluación: " + disponible.ToString("0.##") + " %.");

            evaluacion.Nombre = nombre.Trim();
            evaluacion.Fecha = fecha.Date;
            evaluacion.Ponderacion = ponderacion;
        }

        /// <summary>
        /// Quita una evaluación de la asignatura (la base de datos solo la elimina si no tiene notas).
        /// </summary>
        public void QuitarEvaluacion(int evaluacionId)
        {
            Evaluaciones.RemoveAll(e => e.Id == evaluacionId);
        }

        public decimal SumaPonderaciones()
        {
            return Evaluaciones.Sum(e => e.Ponderacion);
        }

        public override string ToString()
        {
            return Codigo + " - " + Nombre + " (" + Curso + ")";
        }
    }
}