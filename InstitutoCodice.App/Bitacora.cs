using System;
using System.Diagnostics;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Modelos;

namespace InstitutoCodice.App
{
    /// <summary>
    /// Punto único para registrar desde la interfaz los eventos que NO modifican datos:
    /// ingresos, cierres de sesión, cambios de contraseña y exportaciones.
    /// Por defecto usa el usuario de la sesión actual.
    ///
    /// Si la bitácora falla (por ejemplo, un corte de conexión), el error se anota en la
    /// salida de depuración y la aplicación continúa: un problema de registro no debe
    /// impedir que el usuario trabaje.
    ///
    /// Los cambios de datos (estudiantes, docentes, asignaturas, inscripciones, evaluaciones
    /// y notas) NO pasan por aquí: cada repositorio los audita dentro de la misma transacción
    /// que el cambio, así que se guardan los dos o ninguno.
    /// </summary>
    public static class Bitacora
    {
        private static readonly AuditoriaRepositorio repositorio = new AuditoriaRepositorio();

        public static void Registrar(string accion, string? detalle = null,
                                     string? entidad = null, int? entidadId = null,
                                     string? valorAnterior = null, string? valorNuevo = null,
                                     Usuario? usuario = null, string? nombreUsuario = null)
        {
            Usuario? autor = usuario ?? Sesion.UsuarioActual;

            try
            {
                repositorio.Registrar(new RegistroAuditoria
                {
                    UsuarioId = autor?.Id,
                    NombreUsuario = autor?.NombreUsuario ?? nombreUsuario ?? "(desconocido)",
                    Accion = accion,
                    Detalle = detalle,
                    Entidad = entidad,
                    EntidadId = entidadId,
                    ValorAnterior = valorAnterior,
                    ValorNuevo = valorNuevo
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("No se pudo registrar en la bitácora: " + ex.Message);
            }
        }
    }
}