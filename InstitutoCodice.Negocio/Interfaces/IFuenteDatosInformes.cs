using System.Collections.Generic;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Negocio.Interfaces
{
    /// <summary>
    /// Contrato de datos utilizado por el servicio de informes para obtener información académica.
    /// </summary>
    public interface IFuenteDatosInformes
    {
        List<Calificacion> ObtenerCalificaciones(int estudianteId, int asignaturaId);
        List<Asistencia> ObtenerAsistencias(int estudianteId, int asignaturaId);
        List<Asignatura> ObtenerAsignaturasDeEstudiante(int estudianteId);
        List<Estudiante> ObtenerInscritos(int asignaturaId);
    }
}