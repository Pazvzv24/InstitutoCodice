using System;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Aviso de privacidad del sistema (Ley 21.719, deber de información).
    /// Informa a los titulares quién trata sus datos, cuáles, para qué,
    /// quién accede, cuánto tiempo se conservan y cómo ejercer sus derechos.
    ///
    /// El detalle técnico está en docs/RegistroTratamiento.md: si cambia el
    /// tratamiento de datos, se actualizan ambos y se sube la Version.
    /// </summary>
    public static class AvisoPrivacidad
    {
        public const string Version = "1.0";
        public const string Fecha = "noviembre de 2026";
        public const string Titulo = "Aviso de privacidad";

        private static readonly string[] Lineas =
        {
            "AVISO DE PRIVACIDAD",
            "Instituto Códice - Sistema de Asistencia y Notas",
            "Versión " + Version + " - " + Fecha,
            "",
            "1. RESPONSABLE",
            "El Instituto Códice es el responsable del tratamiento de los datos personales registrados en este sistema.",
            "",
            "2. QUÉ DATOS TRATAMOS",
            "- Estudiantes: RUT, nombres, apellidos, correo electrónico (opcional), curso, inscripciones, asistencia y calificaciones.",
            "- Docentes y administradores: nombre, RUT, correo, nombre de usuario y datos de acceso.",
            "Las contraseñas se guardan protegidas con un algoritmo irreversible (hash): nadie, ni siquiera el administrador, puede conocerlas.",
            "",
            "3. PARA QUÉ LOS USAMOS",
            "- Gestionar la vida académica: inscripciones, asistencia, calificaciones e informes de rendimiento.",
            "- El correo del estudiante se usa para comunicaciones académicas de la institución (avisos, citaciones y resultados). Este sistema no envía correos.",
            "- Proteger las cuentas y registrar en una bitácora quién ingresa y qué cambia, para detectar usos indebidos.",
            "No usamos los datos para publicidad ni para elaborar perfiles.",
            "",
            "4. QUIÉN PUEDE VERLOS",
            "- Los administradores académicos.",
            "- Cada docente, solo los datos de los estudiantes de las asignaturas que dicta.",
            "Los datos no se comunican a terceros ni se transfieren fuera del país.",
            "",
            "5. CUÁNTO TIEMPO SE GUARDAN",
            "Mientras dure la relación académica y, después, durante el plazo de conservación de los registros académicos que establezca la institución conforme a la ley. Los registros no se borran: se desactivan, para conservar el historial.",
            "",
            "6. SUS DERECHOS",
            "De acuerdo con la Ley N° 21.719, usted puede solicitar el acceso, la rectificación, la supresión, la oposición, la portabilidad y el bloqueo de sus datos personales.",
            "Para ejercerlos, diríjase a la Dirección Académica del instituto.",
            "",
            "7. SEGURIDAD",
            "El acceso requiere usuario y contraseña, las cuentas se bloquean ante intentos fallidos, cada usuario ve solo lo que su rol le permite y las acciones quedan registradas en una bitácora que no se puede modificar."
        };

        /// <summary>
        /// Texto completo del aviso, listo para mostrar en pantalla.
        /// </summary>
        public static string Texto => string.Join(Environment.NewLine, Lineas);
    }
}