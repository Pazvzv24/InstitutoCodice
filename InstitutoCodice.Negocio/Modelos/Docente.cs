using System.Collections.Generic;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Usuario con rol docente, responsable de las asignaturas asignadas.
    /// </summary>
    public class Docente : Usuario
    {
        public const string Rol = "Docente";

        public string Rut { get; set; }
        public string Correo { get; set; }
        public List<Asignatura> Asignaturas { get; set; }

        public Docente()
        {
            Asignaturas = new List<Asignatura>();
        }

        public override string ObtenerRol()
        {
            return Rol;
        }

        public List<Asignatura> ObtenerAsignaturas()
        {
            return Asignaturas;
        }
    }
}
