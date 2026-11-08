using System;
using System.Collections.Generic;
using System.Linq;

namespace InstitutoCodice.Negocio.Servicios
{
    /// <summary>
    /// Un campo que se compara antes y después de un cambio, para la bitácora.
    /// Si es un dato personal (nombre, correo, RUT de un docente), la bitácora registra
    /// QUÉ campo cambió, pero no su valor (minimización de datos, Ley 21.719).
    /// </summary>
    public sealed class CampoAuditado
    {
        public CampoAuditado(string nombre, string? anterior, string? nuevo, bool esDatoPersonal = false)
        {
            Nombre = nombre;
            Anterior = Normalizar(anterior);
            Nuevo = Normalizar(nuevo);
            EsDatoPersonal = esDatoPersonal;
        }

        public string Nombre { get; }
        public string Anterior { get; }
        public string Nuevo { get; }
        public bool EsDatoPersonal { get; }

        /// <summary>
        /// Vacío y nulo se consideran iguales (por ejemplo, un correo opcional sin valor).
        /// </summary>
        public bool Cambio => !string.Equals(Anterior, Nuevo, StringComparison.Ordinal);

        private static string Normalizar(string? valor) => (valor ?? string.Empty).Trim();
    }

    /// <summary>
    /// Resultado de comparar los campos de un registro antes y después de guardarlo.
    /// </summary>
    public sealed class ResumenCambios
    {
        public ResumenCambios(IReadOnlyList<string> camposModificados, string? valorAnterior, string? valorNuevo)
        {
            CamposModificados = camposModificados;
            ValorAnterior = valorAnterior;
            ValorNuevo = valorNuevo;
        }

        /// <summary>Nombres de todos los campos que cambiaron, incluidos los datos personales.</summary>
        public IReadOnlyList<string> CamposModificados { get; }

        /// <summary>Valores anteriores, solo de los campos que no son datos personales. Null si no hay.</summary>
        public string? ValorAnterior { get; }

        /// <summary>Valores nuevos, solo de los campos que no son datos personales. Null si no hay.</summary>
        public string? ValorNuevo { get; }

        public bool HayCambios => CamposModificados.Count > 0;

        /// <summary>Ejemplo: "Correo, Curso".</summary>
        public string TextoCampos => string.Join(", ", CamposModificados);
    }

    /// <summary>
    /// Compara los campos de un registro para auditar sus modificaciones.
    /// Es lógica pura (sin base de datos), por eso se puede probar con pruebas unitarias.
    /// </summary>
    public static class CambiosAuditoria
    {
        public static ResumenCambios Resumir(params CampoAuditado[] campos)
        {
            List<CampoAuditado> modificados = campos.Where(c => c.Cambio).ToList();
            List<CampoAuditado> visibles = modificados.Where(c => !c.EsDatoPersonal).ToList();

            string? anterior = visibles.Count == 0 ? null : string.Join("; ", visibles.Select(c => c.Nombre + ": " + c.Anterior));
            string? nuevo = visibles.Count == 0 ? null : string.Join("; ", visibles.Select(c => c.Nombre + ": " + c.Nuevo));

            return new ResumenCambios(modificados.Select(c => c.Nombre).ToList(), anterior, nuevo);
        }
    }
}