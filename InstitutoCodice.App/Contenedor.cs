using System;
using Microsoft.Extensions.DependencyInjection;
using InstitutoCodice.Datos.Repositorios;
using InstitutoCodice.Negocio.Interfaces;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.App
{
    /// <summary>
    /// Raíz de composición (composition root): el ÚNICO lugar de la aplicación
    /// donde se decide qué clase concreta se usa para cada dependencia.
    ///
    /// Los formularios no crean sus repositorios con "new": los reciben por el
    /// constructor. Así cada formulario declara lo que necesita, y cambiar una
    /// implementación (por ejemplo, otra fuente de datos) se hace solo aquí.
    /// </summary>
    internal static class Contenedor
    {
        private static IServiceProvider? servicios;

        /// <summary>
        /// Registra las dependencias. Se llama una vez al iniciar la app.
        /// </summary>
        public static void Configurar()
        {
            ServiceCollection registro = new ServiceCollection();

            // Capa de datos (repositorios de SQL Server)
            registro.AddTransient<UsuarioRepositorio>();
            registro.AddTransient<DocenteRepositorio>();
            registro.AddTransient<EstudianteRepositorio>();
            registro.AddTransient<AsignaturaRepositorio>();
            registro.AddTransient<InscripcionRepositorio>();
            registro.AddTransient<AsistenciaRepositorio>();
            registro.AddTransient<EvaluacionRepositorio>();
            registro.AddTransient<CalificacionRepositorio>();
            registro.AddTransient<AuditoriaRepositorio>();
            registro.AddTransient<InformeRepositorio>();

            // Contrato -> implementación: el servicio de informes pide la interfaz,
            // aquí se decide que la cumple el repositorio de SQL Server
            registro.AddTransient<IFuenteDatosInformes>(proveedor => proveedor.GetRequiredService<InformeRepositorio>());

            // Capa de negocio
            registro.AddTransient<ServicioInformes>();
            registro.AddTransient<ServicioDatosTitular>();

            // ValidateOnBuild: si falta registrar algo, la app avisa al iniciar
            // y no recién cuando el usuario abre la pantalla
            servicios = registro.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }

        /// <summary>
        /// Crea un formulario entregándole sus dependencias por el constructor.
        /// Los formularios no se registran en el contenedor: así el contenedor no
        /// guarda referencias a ventanas ya cerradas (no hay fugas de memoria) y
        /// cada formulario se libera con su using como siempre.
        /// </summary>
        /// <param name="argumentos">
        /// Valores que no vienen del contenedor (por ejemplo, el modo "obligatorio"
        /// de FormCambiarContrasena). Se asignan al parámetro del constructor de su mismo tipo.
        /// </param>
        public static T Crear<T>(params object[] argumentos) where T : class
        {
            if (servicios == null)
                throw new InvalidOperationException("El contenedor no está configurado. Llame a Contenedor.Configurar() al iniciar.");

            return ActivatorUtilities.CreateInstance<T>(servicios, argumentos);
        }
    }
}