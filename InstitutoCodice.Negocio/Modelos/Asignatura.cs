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
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Curso { get; set; }
        public Docente Docente { get; set; }
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