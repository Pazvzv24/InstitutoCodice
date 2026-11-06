using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using InstitutoCodice.Negocio.Interfaces;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Derechos de acceso y portabilidad (Ley 21.719): reúne TODOS los datos que el
    /// sistema guarda de un estudiante y los entrega en JSON, un formato estructurado
    /// que puede leer otro sistema.
    ///
    /// Solo incluye datos del titular: no incluye datos de otros estudiantes ni
    /// el nombre de los docentes (son datos personales de terceros).
    /// Funciona aunque el tratamiento esté bloqueado: el derecho de acceso sigue vigente.
    /// </summary>
    public class ServicioDatosTitular
    {
        public const string Formato = "InstitutoCodice.DatosTitular";
        public const string VersionFormato = "1.0";

        private readonly IFuenteDatosInformes fuente;

        public ServicioDatosTitular(IFuenteDatosInformes fuente)
        {
            this.fuente = fuente ?? throw new ArgumentNullException(nameof(fuente));
        }

        /// <summary>
        /// Arma la exportación completa de un estudiante.
        /// </summary>
        public ExportacionTitular Generar(Estudiante estudiante, DateTime generadoUtc)
        {
            if (estudiante == null)
                throw new ArgumentNullException(nameof(estudiante));

            var exportacion = new ExportacionTitular
            {
                GeneradoUtc = DateTime.SpecifyKind(generadoUtc, DateTimeKind.Utc),
                Titular = new TitularExportado
                {
                    Rut = estudiante.Rut ?? "",
                    Nombres = estudiante.Nombres ?? "",
                    Apellidos = estudiante.Apellidos ?? "",
                    Correo = estudiante.Correo ?? "",
                    Curso = estudiante.Curso ?? "",
                    Activo = estudiante.Activo,
                    TratamientoBloqueado = estudiante.TratamientoBloqueado
                }
            };

            foreach (Asignatura asignatura in fuente.ObtenerAsignaturasDeEstudiante(estudiante.Id))
            {
                var exportada = new AsignaturaExportada
                {
                    Codigo = asignatura.Codigo ?? "",
                    Nombre = asignatura.Nombre ?? "",
                    Curso = asignatura.Curso ?? ""
                };

                foreach (Calificacion c in fuente.ObtenerCalificaciones(estudiante.Id, asignatura.Id)
                                                 .OrderBy(c => c.Evaluacion?.Fecha))
                {
                    exportada.Notas.Add(new NotaExportada
                    {
                        Evaluacion = c.Evaluacion?.Nombre ?? "",
                        Fecha = c.Evaluacion != null ? FormatoFecha(c.Evaluacion.Fecha) : "",
                        Ponderacion = c.Evaluacion?.Ponderacion ?? 0m,
                        Nota = c.Nota
                    });
                }

                foreach (Asistencia a in fuente.ObtenerAsistencias(estudiante.Id, asignatura.Id)
                                               .OrderBy(a => a.Fecha))
                {
                    exportada.Asistencia.Add(new AsistenciaExportada
                    {
                        Fecha = FormatoFecha(a.Fecha),
                        Estado = a.Estado.ToString()
                    });
                }

                exportacion.Asignaturas.Add(exportada);
            }

            return exportacion;
        }

        /// <summary>
        /// Convierte la exportación a JSON legible (con sangría y tildes sin escapar).
        /// </summary>
        public static string ConvertirAJson(ExportacionTitular exportacion)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(exportacion, opciones);
        }

        /// <summary>
        /// Nombre sugerido para el archivo, por ejemplo: DatosTitular_11111111-1_20261106_1430.json
        /// </summary>
        public static string NombreArchivo(string rut, DateTime fecha)
        {
            return "DatosTitular_" + FormatoInforme.Limpiar(rut) + "_" + fecha.ToString("yyyyMMdd_HHmm") + ".json";
        }

        // Fechas como texto ISO (2026-11-06): sin horas ni zonas horarias que confundan
        private static string FormatoFecha(DateTime fecha) => fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    // ---------------------------------------------------------------- Estructura del JSON

    public class ExportacionTitular
    {
        public string Formato { get; set; } = ServicioDatosTitular.Formato;
        public string VersionFormato { get; set; } = ServicioDatosTitular.VersionFormato;
        public string Responsable { get; set; } = "Instituto Códice";
        public string BaseLegal { get; set; } = "Ley N° 21.719 - derechos de acceso y portabilidad";
        public DateTime GeneradoUtc { get; set; }
        public TitularExportado Titular { get; set; } = new TitularExportado();
        public List<AsignaturaExportada> Asignaturas { get; set; } = new List<AsignaturaExportada>();
    }

    public class TitularExportado
    {
        public string Rut { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Curso { get; set; } = "";
        public bool Activo { get; set; }
        public bool TratamientoBloqueado { get; set; }
    }

    public class AsignaturaExportada
    {
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Curso { get; set; } = "";
        public List<NotaExportada> Notas { get; set; } = new List<NotaExportada>();
        public List<AsistenciaExportada> Asistencia { get; set; } = new List<AsistenciaExportada>();
    }

    public class NotaExportada
    {
        public string Evaluacion { get; set; } = "";
        public string Fecha { get; set; } = "";
        public decimal Ponderacion { get; set; }
        public decimal Nota { get; set; }
    }

    public class AsistenciaExportada
    {
        public string Fecha { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}