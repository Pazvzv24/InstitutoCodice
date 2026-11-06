using System;
using System.Collections.Generic;
using System.Linq;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa la inscripción de un estudiante en una asignatura.
    /// </summary>
    public class Inscripcion
    {
        public int Id { get; set; }
        public Estudiante Estudiante { get; set; }
        public Asignatura Asignatura { get; set; }
        public DateTime FechaInscripcion { get; set; }

        public Inscripcion()
        {
            FechaInscripcion = DateTime.Today;
        }

        /// <summary>
        /// Verifica si existe una inscripción previa del mismo estudiante en la misma asignatura.
        /// </summary>
        public bool EsDuplicada(List<Inscripcion> lista)
        {
            if (lista == null || Estudiante == null || Asignatura == null)
                return false;

            return lista.Any(i => i.Id != Id
                                  && i.Estudiante != null && i.Asignatura != null
                                  && i.Estudiante.Id == Estudiante.Id
                                  && i.Asignatura.Id == Asignatura.Id);
        }
    }
}