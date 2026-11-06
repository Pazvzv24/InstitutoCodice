namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa el detalle de un estudiante dentro de un informe académico.
    /// </summary>
    public class LineaInforme
    {
        public string Rut { get; set; }
        public string NombreEstudiante { get; set; }
        public string Asignatura { get; set; }
        public string Notas { get; set; }
        public int CantidadNotas { get; set; }
        public decimal Promedio { get; set; }
        public int TotalClases { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
        public string Estado { get; set; }
    }
}
