using System;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa una instancia evaluativa dentro de una asignatura.
    /// </summary>
    public class Evaluacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Ponderacion { get; set; }

        public Evaluacion()
        {
            Fecha = DateTime.Today;
        }

        /// <summary>
        /// Verifica que la ponderación se encuentre dentro del rango permitido.
        /// </summary>
        public bool ValidarPonderacion()
        {
            return Ponderacion > 0 && Ponderacion <= 100;
        }

        public override string ToString()
        {
            return Nombre + " (" + Ponderacion.ToString("0.##") + " %)";
        }
    }
}
