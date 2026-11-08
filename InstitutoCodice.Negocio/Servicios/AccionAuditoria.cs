using System.Collections.Generic;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Tipos de acción que se registran en la bitácora de auditoría,
    /// y reglas simples para decidir qué registrar.
    /// </summary>
    public static class AccionAuditoria
    {
        // Acceso
        public const string LoginExitoso = "LoginExitoso";
        public const string LoginFallido = "LoginFallido";
        public const string LoginBloqueado = "LoginEnCuentaBloqueada";
        public const string CuentaBloqueada = "CuentaBloqueada";
        public const string CierreSesion = "CierreSesion";

        // Contraseñas
        public const string CambioContrasena = "CambioContrasena";
        public const string ContrasenaAsignada = "ContrasenaAsignada";

        // Usuarios y estudiantes
        public const string UsuarioCreado = "UsuarioCreado";
        public const string UsuarioDesactivado = "UsuarioDesactivado";
        public const string UsuarioReactivado = "UsuarioReactivado";
        public const string EstudianteDesactivado = "EstudianteDesactivado";
        public const string EstudianteCreado = "EstudianteCreado";
        public const string EstudianteModificado = "EstudianteModificado";
        public const string EstudianteReactivado = "EstudianteReactivado";
        public const string DocenteModificado = "DocenteModificado";

        // Académico
        public const string AsignaturaCreada = "AsignaturaCreada";
        public const string AsignaturaModificada = "AsignaturaModificada";
        public const string InscripcionCreada = "InscripcionCreada";
        public const string InscripcionEliminada = "InscripcionEliminada";
        public const string EvaluacionCreada = "EvaluacionCreada";
        public const string EvaluacionModificada = "EvaluacionModificada";
        public const string EvaluacionEliminada = "EvaluacionEliminada";
        public const string NotaRegistrada = "NotaRegistrada";
        public const string NotaModificada = "NotaModificada";
        public const string NotaEliminada = "NotaEliminada";

        // Datos personales que salen del sistema
        public const string InformeExportado = "InformeExportado";
        public const string DatosTitularExportados = "DatosTitularExportados"; // Ley 21.719: acceso y portabilidad
        public const string TratamientoBloqueado = "TratamientoBloqueado";     // Ley 21.719: bloqueo
        public const string TratamientoDesbloqueado = "TratamientoDesbloqueado";

        /// <summary>
        /// Todas las acciones, para el filtro de la pantalla de consulta.
        /// </summary>
        public static readonly IReadOnlyList<string> Todas = new[]
        {
            LoginExitoso, LoginFallido, LoginBloqueado, CuentaBloqueada, CierreSesion,
            CambioContrasena, ContrasenaAsignada,
            UsuarioCreado, UsuarioDesactivado, UsuarioReactivado, DocenteModificado,
            EstudianteCreado, EstudianteModificado, EstudianteDesactivado, EstudianteReactivado,
            AsignaturaCreada, AsignaturaModificada, InscripcionCreada, InscripcionEliminada,
            EvaluacionCreada, EvaluacionModificada, EvaluacionEliminada,
            NotaRegistrada, NotaModificada, NotaEliminada,
            InformeExportado, DatosTitularExportados, TratamientoBloqueado, TratamientoDesbloqueado
        };

        /// <summary>
        /// Decide qué registrar al guardar una nota:
        /// sin nota anterior = NotaRegistrada; nota distinta = NotaModificada; misma nota = nada (null).
        /// </summary>
        public static string? AccionParaNota(decimal? notaAnterior, decimal notaNueva)
        {
            if (!notaAnterior.HasValue)
                return NotaRegistrada;

            return notaAnterior.Value != notaNueva ? NotaModificada : null;
        }

        /// <summary>
        /// Arma el detalle legible de un cambio de nota para la bitácora.
        /// Usa el RUT del estudiante; si no se encontró, deja el Id como respaldo.
        /// Ejemplo: Evaluación "Prueba 1" (Id 3), estudiante RUT 20456789-1
        /// </summary>
        public static string DetalleNota(string? nombreEvaluacion, int evaluacionId, string? rutEstudiante, int estudianteId)
        {
            string estudiante = string.IsNullOrWhiteSpace(rutEstudiante)
                ? "estudiante Id " + estudianteId
                : "estudiante RUT " + rutEstudiante.Trim();

            return "Evaluación \"" + nombreEvaluacion + "\" (Id " + evaluacionId + "), " + estudiante;
        }

        /// <summary>
        /// Recorta un texto al largo máximo de su columna en la base de datos.
        /// </summary>
        public static string? Truncar(string? texto, int largoMaximo)
        {
            if (texto == null || texto.Length <= largoMaximo)
                return texto;

            return texto.Substring(0, largoMaximo);
        }
    }
}