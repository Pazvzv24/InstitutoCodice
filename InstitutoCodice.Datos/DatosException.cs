using System;

namespace InstitutoCodice.Datos
{
    /// <summary>
    /// Excepción de acceso a datos con mensaje apto para mostrar al usuario.
    /// </summary>
    public class DatosException : Exception
    {
        public DatosException(string mensaje, Exception interna) : base(mensaje, interna)
        {
        }
    }
}
