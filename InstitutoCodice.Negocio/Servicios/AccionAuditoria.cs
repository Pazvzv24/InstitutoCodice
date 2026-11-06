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

        // Académico
        public const string EvaluacionCreada = "EvaluacionCreada";
        public const string NotaRegistrada = "NotaRegistrada";
        public const string NotaModificada = "NotaModificada";

        // Datos personales que salen del sistema
        public const string InformeExportado = "InformeExportado";

        /// <summary>
        /// Todas las acciones, para el filtro de la pantalla de consulta.
        /// </summary>
        public static readonly IReadOnlyList<string> Todas = new[]
        {
            LoginExitoso, LoginFallido, LoginBloqueado, CuentaBloqueada, CierreSesion,
            CambioContrasena, ContrasenaAsignada,
            UsuarioCreado, UsuarioDesactivado, UsuarioReactivado, EstudianteDesactivado,
            EvaluacionCreada, NotaRegistrada, NotaModificada,
            InformeExportado
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