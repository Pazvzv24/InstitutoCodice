using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstitutoCodice.Negocio.Interfaces;
using InstitutoCodice.Negocio.Modelos;
using InstitutoCodice.Negocio.Servicios;

namespace InstitutoCodice.Pruebas
{
    // Pruebas de negocio - 3C / C.E. 2.18 - Patron AAA
    [TestClass]
    public class PruebasSistema
    {
        // RF-01: RUT
        [TestMethod]
        public void AgregarEstudiante_ConRutValido_EsAceptado()
        {
            var estudiante = new Estudiante
            {
                Rut = "12345678-5",
                Nombres = "Camila",
                Apellidos = "González"
            };

            bool resultado = estudiante.ValidarRut();

            Assert.IsTrue(resultado, "12345678-5 es valido, deberia pasar.");
        }

        [TestMethod]
        public void AgregarEstudiante_ConRutInvalido_EsRechazado()
        {
            var estudiante = new Estudiante { Rut = "12345678-4" }; // DV malo

            bool resultado = estudiante.ValidarRut();

            Assert.IsFalse(resultado, "DV incorrecto, debe fallar.");
        }

        // RF-05: Asistencia sin fecha futura
        [TestMethod]
        public void RegistrarAsistencia_ConFechaDeHoy_EsValida()
        {
            var asistencia = new Asistencia
            {
                Fecha = DateTime.Today
            };
            asistencia.CambiarEstado(EstadoAsistencia.Presente);

            Assert.IsTrue(asistencia.ValidarFecha());
            Assert.AreEqual(EstadoAsistencia.Presente, asistencia.Estado);
        }

        [TestMethod]
        public void RegistrarAsistencia_ConFechaFutura_EsRechazada()
        {
            var asistencia = new Asistencia { Fecha = DateTime.Today.AddDays(1) };

            bool resultado = asistencia.ValidarFecha();

            Assert.IsFalse(resultado, "Fecha futura no permitida.");
        }

        // RF-07 / RF-08: Notas 1.0 - 7.0
        [TestMethod]
        public void IngresarNota_FueraDeRango_EsRechazada()
        {
            var notaAlta = new Calificacion { Nota = 7.5m };
            var notaBaja = new Calificacion { Nota = 0.5m };
            var notaOk = new Calificacion { Nota = 5.5m };

            Assert.IsFalse(notaAlta.ValidarNota(), "7.5 fuera de rango.");
            Assert.IsFalse(notaBaja.ValidarNota(), "0.5 fuera de rango.");
            Assert.IsTrue(notaOk.ValidarNota(), "5.5 debe pasar.");
        }

        [TestMethod]
        public void ModificarNota_FueraDeRango_LanzaExcepcion()
        {
            var calificacion = new Calificacion { Nota = 5.0m };
            bool lanzoExcepcion = false;

            try
            {
                calificacion.ModificarNota(8.0m);
            }
            catch (ArgumentOutOfRangeException)
            {
                lanzoExcepcion = true;
            }

            Assert.IsTrue(lanzoExcepcion, "Modificar a 8.0 debe tirar excepcion.");
            Assert.AreEqual(5.0m, calificacion.Nota, "No debe cambiar la nota original.");
        }

        // RF-07: Ponderaciones max 100%
        [TestMethod]
        public void AgregarEvaluacion_QueSuperaCienPorCiento_EsRechazada()
        {
            var asignatura = new Asignatura();
            asignatura.AgregarEvaluacion(CrearEvaluacion("Prueba 1", 60m));

            bool lanzoExcepcion = false;
            try
            {
                asignatura.AgregarEvaluacion(CrearEvaluacion("Prueba 2", 50m)); // 110%
            }
            catch (InvalidOperationException)
            {
                lanzoExcepcion = true;
            }

            Assert.IsTrue(lanzoExcepcion, "No puede superar 100%.");
            Assert.AreEqual(60m, asignatura.SumaPonderaciones());
        }

        // RF-09: Promedio y asistencia
        [TestMethod]
        public void CalcularPromedio_Ponderado_DevuelveResultadoCorrecto()
        {
            // 6.0*30% + 6.3*30% + 6.5*40% = 6.29 -> 6.3
            var notas = new List<Calificacion>
            {
                CrearCalificacion(6.0m, 30m),
                CrearCalificacion(6.3m, 30m),
                CrearCalificacion(6.5m, 40m)
            };

            decimal promedio = ServicioInformes.CalcularPromedioPonderado(notas);

            Assert.AreEqual(6.3m, promedio);
        }

        [TestMethod]
        public void CalcularPorcentajeAsistencia_DevuelveResultadoCorrecto()
        {
            // 3 de 4 = 75%
            var asistencias = new List<Asistencia>
            {
                CrearAsistencia(EstadoAsistencia.Presente),
                CrearAsistencia(EstadoAsistencia.Presente),
                CrearAsistencia(EstadoAsistencia.Ausente),
                CrearAsistencia(EstadoAsistencia.Presente)
            };

            decimal porcentaje = ServicioInformes.CalcularPorcentaje(asistencias);

            Assert.AreEqual(75.0m, porcentaje);
        }

        // RF-10: Aprobado / Reprobado
        [TestMethod]
        public void ObtenerEstado_ConBuenPromedioPeroPocaAsistencia_EsReprobado()
        {
            var servicio = new ServicioInformes(new FuenteDatosFalsa());

            Assert.AreEqual(ServicioInformes.Aprobado, servicio.ObtenerEstado(5.0m, 80m));
            Assert.AreEqual(ServicioInformes.Reprobado, servicio.ObtenerEstado(6.3m, 66.7m)); // <75% asistencia
            Assert.AreEqual(ServicioInformes.Reprobado, servicio.ObtenerEstado(3.9m, 100m)); // <4.0 promedio
        }

        // RF-12 / RNF-03: Autenticación y hash de contraseñas (PBKDF2)
        [TestMethod]
        public void VerificarContrasena_ConClaveCorrecta_EsAceptada()
        {
            string hash = Seguridad.GenerarHash("Admin123");

            bool resultado = Seguridad.VerificarContrasena("Admin123", hash);

            Assert.IsTrue(resultado, "La clave correcta debe validar.");
            StringAssert.StartsWith(hash, "PBKDF2$", "El hash debe usar el formato versionado.");
        }

        [TestMethod]
        public void VerificarContrasena_ConClaveIncorrectaOMayusculasDistintas_EsRechazada()
        {
            string hash = Seguridad.GenerarHash("Admin123");

            Assert.IsFalse(Seguridad.VerificarContrasena("Otra456", hash), "Clave distinta debe fallar.");
            Assert.IsFalse(Seguridad.VerificarContrasena("admin123", hash), "Debe distinguir mayúsculas.");
        }

        [TestMethod]
        public void GenerarHash_MismaClaveDosVeces_ProduceHashesDistintos()
        {
            string hash1 = Seguridad.GenerarHash("Docente123");
            string hash2 = Seguridad.GenerarHash("Docente123");

            Assert.AreNotEqual(hash1, hash2, "La sal aleatoria debe generar hashes distintos.");
            Assert.IsTrue(Seguridad.VerificarContrasena("Docente123", hash1));
            Assert.IsTrue(Seguridad.VerificarContrasena("Docente123", hash2));
        }

        [TestMethod]
        public void VerificarContrasena_ConHashInvalidoOVacio_DevuelveFalseSinExcepcion()
        {
            Assert.IsFalse(Seguridad.VerificarContrasena("Admin123", null));
            Assert.IsFalse(Seguridad.VerificarContrasena("Admin123", "texto-cualquiera"));
            Assert.IsFalse(Seguridad.VerificarContrasena("Admin123", "PBKDF2$abc$###$###"));
            Assert.IsFalse(Seguridad.VerificarContrasena("", Seguridad.GenerarHash("Admin123")));
        }

        // RF-10: Resultado parcial vs definitivo
        [TestMethod]
        public void EsDefinitivo_ConEvaluacionesPendientes_EsParcial()
        {
            var incompletas = new List<Calificacion> { CrearCalificacion(5.5m, 50m) };           // 50%
            var completas = new List<Calificacion> { CrearCalificacion(5.5m, 50m), CrearCalificacion(6.0m, 50m) }; // 100%

            Assert.IsFalse(ServicioInformes.EsDefinitivo(incompletas), "Con 50% evaluado es parcial.");
            Assert.IsTrue(ServicioInformes.EsDefinitivo(completas), "Con 100% evaluado es definitivo.");
        }

        // RF-12 / RNF-03: Bloqueo de cuenta por intentos fallidos
        private static readonly DateTime Ahora = new DateTime(2026, 11, 6, 12, 0, 0, DateTimeKind.Utc);

        [TestMethod]
        public void RegistrarFallo_TresVeces_BloqueaLaCuenta()
        {
            var usuario = new Administrador();

            bool bloqueo1 = PoliticaBloqueo.RegistrarFallo(usuario, Ahora);
            bool bloqueo2 = PoliticaBloqueo.RegistrarFallo(usuario, Ahora);
            bool bloqueo3 = PoliticaBloqueo.RegistrarFallo(usuario, Ahora);

            Assert.IsFalse(bloqueo1, "1er fallo no bloquea.");
            Assert.IsFalse(bloqueo2, "2do fallo no bloquea.");
            Assert.IsTrue(bloqueo3, "3er fallo debe bloquear.");
            Assert.IsTrue(PoliticaBloqueo.EstaBloqueado(usuario, Ahora));
            Assert.AreEqual(15, PoliticaBloqueo.MinutosRestantes(usuario, Ahora));
        }

        [TestMethod]
        public void EstaBloqueado_DespuesDe15Minutos_SeLevantaElBloqueo()
        {
            var usuario = new Administrador();
            for (int i = 0; i < PoliticaBloqueo.MaxIntentos; i++)
                PoliticaBloqueo.RegistrarFallo(usuario, Ahora);

            Assert.IsTrue(PoliticaBloqueo.EstaBloqueado(usuario, Ahora.AddMinutes(14)), "A los 14 min sigue bloqueada.");
            Assert.IsFalse(PoliticaBloqueo.EstaBloqueado(usuario, Ahora.AddMinutes(16)), "A los 16 min ya no.");
        }

        [TestMethod]
        public void RegistrarFallo_DespuesDeBloqueoExpirado_ReiniciaElContador()
        {
            var usuario = new Administrador();
            for (int i = 0; i < PoliticaBloqueo.MaxIntentos; i++)
                PoliticaBloqueo.RegistrarFallo(usuario, Ahora);

            bool bloqueo = PoliticaBloqueo.RegistrarFallo(usuario, Ahora.AddMinutes(20));

            Assert.IsFalse(bloqueo, "Tras expirar el bloqueo, un fallo no vuelve a bloquear.");
            Assert.AreEqual(1, usuario.IntentosFallidos, "El contador parte de cero.");
        }

        [TestMethod]
        public void RegistrarExito_ReiniciaContadorYBloqueo()
        {
            var usuario = new Administrador();
            PoliticaBloqueo.RegistrarFallo(usuario, Ahora);
            PoliticaBloqueo.RegistrarFallo(usuario, Ahora);

            PoliticaBloqueo.RegistrarExito(usuario);

            Assert.AreEqual(0, usuario.IntentosFallidos);
            Assert.IsNull(usuario.BloqueadoHasta);
            Assert.IsFalse(PoliticaBloqueo.EstaBloqueado(usuario, Ahora));
        }

        // RNF-03: Política de contraseñas
        [TestMethod]
        public void PoliticaContrasena_ConClaveQueCumpleTodo_EsValida()
        {
            Assert.IsTrue(PoliticaContrasena.EsValida("Segura2026", "pperez"));
        }

        [TestMethod]
        public void PoliticaContrasena_ConClaveDebil_DevuelveCadaReglaIncumplida()
        {
            Assert.IsFalse(PoliticaContrasena.EsValida("Ab1", "pperez"), "Muy corta.");
            Assert.IsFalse(PoliticaContrasena.EsValida("segura2026", "pperez"), "Sin mayúscula.");
            Assert.IsFalse(PoliticaContrasena.EsValida("SEGURA2026", "pperez"), "Sin minúscula.");
            Assert.IsFalse(PoliticaContrasena.EsValida("SeguraSegura", "pperez"), "Sin número.");
            Assert.AreEqual(4, PoliticaContrasena.Validar("", "pperez").Count, "Vacía incumple largo, mayúscula, minúscula y número.");
        }

        [TestMethod]
        public void PoliticaContrasena_QueContieneElUsuario_EsRechazada()
        {
            Assert.IsFalse(PoliticaContrasena.EsValida("Pperez2026", "pperez"), "No distingue mayúsculas al comparar el usuario.");
        }

        [TestMethod]
        public void PoliticaContrasena_IgualALaActual_EsRechazada()
        {
            Assert.IsFalse(PoliticaContrasena.EsValida("Docente123", "pperez", "Docente123"));
            Assert.IsTrue(PoliticaContrasena.EsValida("Docente456", "pperez", "Docente123"));
        }

        // Helpers
        private static Evaluacion CrearEvaluacion(string nombre, decimal ponderacion)
        {
            return new Evaluacion { Nombre = nombre, Ponderacion = ponderacion };
        }

        private static Calificacion CrearCalificacion(decimal nota, decimal ponderacion)
        {
            return new Calificacion
            {
                Nota = nota,
                Evaluacion = CrearEvaluacion("Evaluación", ponderacion)
            };
        }

        private static Asistencia CrearAsistencia(EstadoAsistencia estado)
        {
            return new Asistencia { Estado = estado };
        }
    }

    // Fake para testear ServicioInformes sin BD
    public class FuenteDatosFalsa : IFuenteDatosInformes
    {
        public List<Calificacion> ObtenerCalificaciones(int estudianteId, int asignaturaId) => new List<Calificacion>();
        public List<Asistencia> ObtenerAsistencias(int estudianteId, int asignaturaId) => new List<Asistencia>();
        public List<Asignatura> ObtenerAsignaturasDeEstudiante(int estudianteId) => new List<Asignatura>();
        public List<Estudiante> ObtenerInscritos(int asignaturaId) => new List<Estudiante>();
    }
}