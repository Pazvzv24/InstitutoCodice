using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Negocio.Modelos
{
    /// <summary>
    /// Representa a un estudiante registrado en el sistema.
    /// </summary>
    public class Estudiante
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Curso { get; set; }
        public bool Activo { get; set; }

        public Estudiante()
        {
            Activo = true;
        }

        /// <summary>
        /// Valida el formato y dígito verificador del RUT.
        /// </summary>
        public bool ValidarRut()
        {
            return ValidadorRut.EsValido(Rut);
        }

        /// <summary>
        /// Desactiva al estudiante manteniendo su historial académico.
        /// </summary>
        public void Desactivar()
        {
            Activo = false;
        }

        public string NombreCompleto()
        {
            return ((Nombres ?? "") + " " + (Apellidos ?? "")).Trim();
        }

        public override string ToString()
        {
            return NombreCompleto() + " (" + Rut + ")";
        }
    }
}
