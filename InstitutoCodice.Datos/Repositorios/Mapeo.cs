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

            // Columna opcional: si la consulta no la trae, se asume no bloqueado
            int iBloqueado = BuscarColumna(lector, "TratamientoBloqueado");
            estudiante.TratamientoBloqueado = iBloqueado >= 0 && lector.GetBoolean(iBloqueado);
            return estudiante;
        }

        /// <summary>
        /// Devuelve la posición de una columna en el resultado, o -1 si la consulta no la incluye.
        /// </summary>
        private static int BuscarColumna(SqlDataReader lector, string nombre)
        {
            for (int i = 0; i < lector.FieldCount; i++)
            {
                if (string.Equals(lector.GetName(i), nombre, System.StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
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