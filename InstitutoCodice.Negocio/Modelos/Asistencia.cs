using System;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa el registro de asistencia de un estudiante en una asignatura.
    /// </summary>
    public class Asistencia
    {
        public int Id { get; set; }
        public Estudiante Estudiante { get; set; }
        public Asignatura Asignatura { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoAsistencia Estado { get; set; }

        public Asistencia()
        {
            Fecha = DateTime.Today;
            Estado = EstadoAsistencia.Presente;
        }

        /// <summary>
        /// Verifica que la fecha no sea posterior al día actual.
        /// </summary>
        public bool ValidarFecha()
        {
            return Fecha.Date <= DateTime.Today;
        }

        /// <summary>
        /// Actualiza el estado de asistencia registrado.
        /// </summary>
        public void CambiarEstado(EstadoAsistencia nuevo)
        {
            if (!Enum.IsDefined(typeof(EstadoAsistencia), nuevo))
                throw new ArgumentException("Estado de asistencia no válido.");

            Estado = nuevo;
        }
    }
}