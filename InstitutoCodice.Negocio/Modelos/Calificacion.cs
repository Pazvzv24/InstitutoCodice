using System;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa la calificación obtenida por un estudiante en una evaluación.
    /// </summary>
    public class Calificacion
    {
        public const decimal NotaMinima = 1.0m;
        public const decimal NotaMaxima = 7.0m;

        public int Id { get; set; }
        public Estudiante Estudiante { get; set; }
        public Evaluacion Evaluacion { get; set; }
        public decimal Nota { get; set; }

        /// <summary>
        /// Verifica que la nota se encuentre dentro del rango permitido.
        /// </summary>
        public bool ValidarNota()
        {
            return Nota >= NotaMinima && Nota <= NotaMaxima;
        }

        /// <summary>
        /// Actualiza el valor de la calificación aplicando validación de rango.
        /// </summary>
        public void ModificarNota(decimal nueva)
        {
            if (nueva < NotaMinima || nueva > NotaMaxima)
                throw new ArgumentOutOfRangeException("nueva", "La nota debe estar entre 1,0 y 7,0.");

            Nota = nueva;
        }
    }
}
