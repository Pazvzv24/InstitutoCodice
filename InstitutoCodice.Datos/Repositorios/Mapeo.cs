using Microsoft.Data.SqlClient;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Datos.Repositorios
{
    /// <summary>
    /// Funciones de mapeo para transformar registros de base de datos en entidades del dominio.
    /// </summary>
    internal static class Mapeo
    {
        public static Estudiante LeerEstudiante(SqlDataReader lector)
        {
            Estudiante estudiante = new Estudiante();
            estudiante.Id = lector.GetInt32(lector.GetOrdinal("Id"));
            estudiante.Rut = lector.GetString(lector.GetOrdinal("Rut"));
            estudiante.Nombres = lector.GetString(lector.GetOrdinal("Nombres"));
            estudiante.Apellidos = lector.GetString(lector.GetOrdinal("Apellidos"));
            int iCorreo = lector.GetOrdinal("Correo");
            estudiante.Correo = lector.IsDBNull(iCorreo) ? "" : lector.GetString(iCorreo);
            estudiante.Curso = lector.GetString(lector.GetOrdinal("Curso"));
            estudiante.Activo = lector.GetBoolean(lector.GetOrdinal("Activo"));
            return estudiante;
        }

        public static Asignatura LeerAsignatura(SqlDataReader lector)
        {
            Asignatura asignatura = new Asignatura();
            asignatura.Id = lector.GetInt32(lector.GetOrdinal("Id"));
            asignatura.Codigo = lector.GetString(lector.GetOrdinal("Codigo"));
            asignatura.Nombre = lector.GetString(lector.GetOrdinal("Nombre"));
            asignatura.Curso = lector.GetString(lector.GetOrdinal("Curso"));

            Docente docente = new Docente();
            docente.Id = lector.GetInt32(lector.GetOrdinal("DocenteId"));
            docente.Nombre = lector.GetString(lector.GetOrdinal("NombreDocente"));
            asignatura.Docente = docente;
            return asignatura;
        }

        public static Evaluacion LeerEvaluacion(SqlDataReader lector)
        {
            Evaluacion evaluacion = new Evaluacion();
            evaluacion.Id = lector.GetInt32(lector.GetOrdinal("EvaluacionId"));
            evaluacion.Nombre = lector.GetString(lector.GetOrdinal("NombreEvaluacion"));
            evaluacion.Fecha = lector.GetDateTime(lector.GetOrdinal("FechaEvaluacion"));
            evaluacion.Ponderacion = lector.GetDecimal(lector.GetOrdinal("Ponderacion"));
            return evaluacion;
        }
    }
}
