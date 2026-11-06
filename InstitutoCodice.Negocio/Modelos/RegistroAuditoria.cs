using System;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Un registro de la bitácora de auditoría: quién hizo qué, cuándo y sobre qué dato.
    /// Nunca contiene contraseñas.
    /// </summary>
    public class RegistroAuditoria
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; }
        public int? UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string? Detalle { get; set; }
        public string? Entidad { get; set; }
        public int? EntidadId { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }
}