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

        // Auditoría: qué se registra al guardar una nota
        [TestMethod]
        public void AccionParaNota_DistingueRegistroModificacionYSinCambio()
        {
            Assert.AreEqual(AccionAuditoria.NotaRegistrada, AccionAuditoria.AccionParaNota(null, 5.5m), "Sin nota previa = registro.");
            Assert.AreEqual(AccionAuditoria.NotaModificada, AccionAuditoria.AccionParaNota(4.0m, 5.5m), "Nota distinta = modificación.");
            Assert.IsNull(AccionAuditoria.AccionParaNota(5.5m, 5.5m), "Misma nota = no se audita.");
        }

        [TestMethod]
        public void Truncar_RecortaTextosLargosSinTocarLosCortos()
        {
            Assert.AreEqual("abc", AccionAuditoria.Truncar("abcdef", 3));
            Assert.AreEqual("ab", AccionAuditoria.Truncar("ab", 3));
            Assert.IsNull(AccionAuditoria.Truncar(null, 3));
        }

        // Auditoría de modificaciones (estudiantes, docentes y asignaturas)
        [TestMethod]
        public void ResumirCambios_SinDiferencias_NoHayCambios()
        {
            // Arrange: mismo curso y correo vacío frente a nulo (el correo es opcional)
            var curso = new CampoAuditado("Curso", "TI-301", "TI-301");
            var correo = new CampoAuditado("Correo", null, "", esDatoPersonal: true);

            // Act
            ResumenCambios resumen = CambiosAuditoria.Resumir(curso, correo);

            // Assert
            Assert.IsFalse(resumen.HayCambios);
            Assert.IsNull(resumen.ValorAnterior);
            Assert.IsNull(resumen.ValorNuevo);
        }

        [TestMethod]
        public void ResumirCambios_DatoNoPersonal_RegistraValorAnteriorYNuevo()
        {
            ResumenCambios resumen = CambiosAuditoria.Resumir(
                new CampoAuditado("RUT", "20456789-1", "20456789-1"),
                new CampoAuditado("Curso", "TI-301", "TI-304"));

            Assert.AreEqual("Curso", resumen.TextoCampos);
            Assert.AreEqual("Curso: TI-301", resumen.ValorAnterior);
            Assert.AreEqual("Curso: TI-304", resumen.ValorNuevo);
        }

        [TestMethod]
        public void ResumirCambios_DatoPersonal_RegistraElCampoPeroNoSuValor()
        {
            // Minimización (Ley 21.719): la bitácora sabe QUE el correo cambió, no cuál era
            ResumenCambios resumen = CambiosAuditoria.Resumir(
                new CampoAuditado("Correo", "camila@codice.cl", "camila.g@codice.cl", esDatoPersonal: true),
                new CampoAuditado("Curso", "TI-301", "TI-302"));

            CollectionAssert.AreEqual(new[] { "Correo", "Curso" }, new List<string>(resumen.CamposModificados));
            Assert.IsFalse(resumen.ValorAnterior!.Contains("camila"), "No debe guardar el correo anterior.");
            Assert.IsFalse(resumen.ValorNuevo!.Contains("camila"), "No debe guardar el correo nuevo.");
            Assert.AreEqual("Curso: TI-301", resumen.ValorAnterior);
        }

        // Exportación: nombres de archivo seguros
        [TestMethod]
        public void NombreArchivo_ConCaracteresNoPermitidos_LosReemplaza()
        {
            var fecha = new DateTime(2026, 11, 6, 14, 30, 0);

            string nombre = FormatoInforme.NombreArchivo("Curso", "POO101: Programación / 2026?", fecha, "pdf");

            Assert.AreEqual("Informe_Curso_POO101_Programación_2026_20261106_1430.pdf", nombre);
            foreach (char c in new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|', ' ' })
                Assert.IsFalse(nombre.Contains(c), "No debe contener '" + c + "'.");
        }

        [TestMethod]
        public void DetalleNota_MuestraRutYUsaIdSiNoHayRut()
        {
            string conRut = AccionAuditoria.DetalleNota("Prueba 1", 3, "20456789-1", 7);
            string sinRut = AccionAuditoria.DetalleNota("Prueba 1", 3, null, 7);

            Assert.AreEqual("Evaluación \"Prueba 1\" (Id 3), estudiante RUT 20456789-1", conRut);
            Assert.AreEqual("Evaluación \"Prueba 1\" (Id 3), estudiante Id 7", sinRut, "Sin RUT usa el Id como respaldo.");
        }

        // Control de acceso: un docente solo ve lo de sus asignaturas
        [TestMethod]
        public void InformePorEstudiante_Docente_SoloVeLasAsignaturasQueDicta()
        {
            // Arrange: el estudiante está en POO101 y BDD201; el docente solo dicta BDD201
            var poo = new Asignatura { Id = 1, Nombre = "Programación Orientada a Objetos" };
            var bdd = new Asignatura { Id = 2, Nombre = "Bases de Datos" };
            var estudiante = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(estudiante, poo, bdd);
            var servicio = new ServicioInformes(fuente);

            // Act
            var lineas = servicio.InformePorEstudiante(estudiante, new List<Asignatura> { bdd });

            // Assert
            Assert.AreEqual(1, lineas.Count, "El docente solo debe ver 1 de las 2 asignaturas.");
            Assert.AreEqual("Bases de Datos", lineas[0].Asignatura, "La asignatura visible debe ser la que dicta.");
        }

        [TestMethod]
        public void InformePorEstudiante_SinAsignaturasPermitidas_NoDevuelveNada()
        {
            var poo = new Asignatura { Id = 1, Nombre = "Programación Orientada a Objetos" };
            var estudiante = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(estudiante, poo);
            var servicio = new ServicioInformes(fuente);

            // Un docente que no dicta ningún curso del estudiante (lista vacía o null) no ve nada
            Assert.AreEqual(0, servicio.InformePorEstudiante(estudiante, new List<Asignatura>()).Count, "Lista vacía: nada visible.");
            Assert.AreEqual(0, servicio.InformePorEstudiante(estudiante, null).Count, "Null: falla cerrado, nada visible.");
        }

        [TestMethod]
        public void ObtenerEstudiantesDe_SoloDevuelveInscritosEnSusCursos_SinRepetir()
        {
            // Arrange: Ana está en POO y BDD, Bruno solo en POO, Carla solo en TPR (curso ajeno)
            var poo = new Asignatura { Id = 1, Nombre = "POO" };
            var bdd = new Asignatura { Id = 2, Nombre = "BDD" };
            var tpr = new Asignatura { Id = 3, Nombre = "TPR" };
            var ana = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };
            var bruno = new Estudiante { Id = 20, Rut = "22222222-2", Nombres = "Bruno", Apellidos = "Soto" };
            var carla = new Estudiante { Id = 30, Rut = "15555555-6", Nombres = "Carla", Apellidos = "Rojas" };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(ana, poo, bdd);
            fuente.Inscribir(bruno, poo);
            fuente.Inscribir(carla, tpr);
            var servicio = new ServicioInformes(fuente);

            // Act: docente que dicta POO y BDD
            var visibles = servicio.ObtenerEstudiantesDe(new List<Asignatura> { poo, bdd });

            // Assert
            Assert.AreEqual(2, visibles.Count, "Ana aparece una sola vez aunque esté en 2 cursos; Carla no aparece.");
            Assert.AreEqual("11111111-1", visibles[0].Rut, "Ordenado por nombre: primero Ana.");
            Assert.AreEqual("22222222-2", visibles[1].Rut, "Luego Bruno.");
        }

        // RF-04: Inscripción duplicada
        [TestMethod]
        public void EsDuplicada_MismoEstudianteYAsignatura_EsDetectada()
        {
            var estudiante = new Estudiante { Id = 10 };
            var poo = new Asignatura { Id = 1 };
            var bdd = new Asignatura { Id = 2 };
            var existentes = new List<Inscripcion>
            {
                new Inscripcion { Id = 100, Estudiante = estudiante, Asignatura = poo }
            };

            var repetida = new Inscripcion { Id = 0, Estudiante = estudiante, Asignatura = poo };
            var otraAsignatura = new Inscripcion { Id = 0, Estudiante = estudiante, Asignatura = bdd };
            var laMisma = new Inscripcion { Id = 100, Estudiante = estudiante, Asignatura = poo };

            Assert.IsTrue(repetida.EsDuplicada(existentes), "Mismo estudiante y asignatura = duplicada.");
            Assert.IsFalse(otraAsignatura.EsDuplicada(existentes), "Otra asignatura no es duplicada.");
            Assert.IsFalse(laMisma.EsDuplicada(existentes), "Una inscripción no se compara consigo misma (al editar).");
        }

        // Ley 21.719: deber de información
        [TestMethod]
        public void AvisoPrivacidad_IncluyeLosElementosQueExigeLaLey()
        {
            string texto = AvisoPrivacidad.Texto;

            StringAssert.Contains(texto, "RESPONSABLE", "Debe indicar quién es el responsable.");
            StringAssert.Contains(texto, "PARA QUÉ LOS USAMOS", "Debe indicar la finalidad.");
            StringAssert.Contains(texto, "correo", "Debe informar para qué se usa el correo.");
            StringAssert.Contains(texto, "QUIÉN PUEDE VERLOS", "Debe indicar quién accede.");
            StringAssert.Contains(texto, "CUÁNTO TIEMPO", "Debe indicar la conservación.");
            StringAssert.Contains(texto, "21.719", "Debe citar la ley.");
            foreach (string derecho in new[] { "acceso", "rectificación", "supresión", "oposición", "portabilidad", "bloqueo" })
                StringAssert.Contains(texto, derecho, "Debe mencionar el derecho de " + derecho + ".");
            StringAssert.Contains(texto, "Versión " + AvisoPrivacidad.Version, "El aviso debe estar versionado.");
        }

        // Ley 21.719: derechos de acceso y portabilidad
        [TestMethod]
        public void DatosTitular_IncluyeSusAsignaturasNotasYAsistencia()
        {
            // Arrange: Ana está en POO (con una nota) y en BDD (con 2 asistencias, ingresadas desordenadas)
            var poo = new Asignatura { Id = 1, Codigo = "POO101", Nombre = "Programación Orientada a Objetos", Curso = "TI-301" };
            var bdd = new Asignatura { Id = 2, Codigo = "BDD201", Nombre = "Bases de Datos", Curso = "TI-301" };
            var ana = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez", Curso = "TI-301" };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(ana, poo, bdd);
            fuente.AgregarNota(ana, poo, new Calificacion
            {
                Nota = 6.5m,
                Evaluacion = new Evaluacion { Nombre = "Prueba 1", Ponderacion = 40m, Fecha = new DateTime(2026, 10, 15) }
            });
            fuente.AgregarAsistencia(ana, bdd, new Asistencia { Fecha = new DateTime(2026, 10, 8), Estado = EstadoAsistencia.Ausente });
            fuente.AgregarAsistencia(ana, bdd, new Asistencia { Fecha = new DateTime(2026, 10, 1), Estado = EstadoAsistencia.Presente });

            var servicio = new ServicioDatosTitular(fuente);

            // Act
            var datos = servicio.Generar(ana, new DateTime(2026, 11, 6, 20, 0, 0));

            // Assert
            Assert.AreEqual("11111111-1", datos.Titular.Rut);
            Assert.AreEqual(2, datos.Asignaturas.Count, "Debe incluir todas sus asignaturas.");

            var notasPoo = datos.Asignaturas.Find(a => a.Codigo == "POO101")!.Notas;
            Assert.AreEqual(1, notasPoo.Count);
            Assert.AreEqual(6.5m, notasPoo[0].Nota);
            Assert.AreEqual("2026-10-15", notasPoo[0].Fecha);

            var asistenciaBdd = datos.Asignaturas.Find(a => a.Codigo == "BDD201")!.Asistencia;
            Assert.AreEqual(2, asistenciaBdd.Count);
            Assert.AreEqual("2026-10-01", asistenciaBdd[0].Fecha, "La asistencia debe venir ordenada por fecha.");
            Assert.AreEqual("Ausente", asistenciaBdd[1].Estado);
        }

        [TestMethod]
        public void DatosTitular_NoIncluyeDatosDeOtrosEstudiantes()
        {
            // Arrange: Ana y Bruno comparten la asignatura POO; Bruno tiene una nota
            var poo = new Asignatura { Id = 1, Codigo = "POO101", Nombre = "POO" };
            var ana = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };
            var bruno = new Estudiante { Id = 20, Rut = "22222222-2", Nombres = "Bruno", Apellidos = "Soto" };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(ana, poo);
            fuente.Inscribir(bruno, poo);
            fuente.AgregarNota(bruno, poo, new Calificacion { Nota = 3.0m, Evaluacion = new Evaluacion { Nombre = "Prueba 1", Ponderacion = 40m } });

            // Act: se exportan los datos de Ana
            string json = ServicioDatosTitular.ConvertirAJson(new ServicioDatosTitular(fuente).Generar(ana, DateTime.UtcNow));

            // Assert
            StringAssert.Contains(json, "11111111-1");
            Assert.IsFalse(json.Contains("22222222-2"), "No debe incluir el RUT de otro estudiante.");
            Assert.IsFalse(json.Contains("Bruno"), "No debe incluir el nombre de otro estudiante.");
            Assert.IsFalse(json.Contains("\"nota\": 3"), "No debe incluir notas de otro estudiante.");
        }

        [TestMethod]
        public void DatosTitular_Json_EsLegibleYCitaLaLey()
        {
            var ana = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };

            string json = ServicioDatosTitular.ConvertirAJson(
                new ServicioDatosTitular(new FuenteDatosConInscripciones()).Generar(ana, DateTime.UtcNow));

            StringAssert.Contains(json, "\"rut\": \"11111111-1\"", "Formato JSON con nombres en camelCase.");
            StringAssert.Contains(json, "Pérez", "Las tildes deben quedar legibles, sin códigos \\u00e9.");
            StringAssert.Contains(json, ServicioDatosTitular.Formato, "Debe identificar el formato.");
            StringAssert.Contains(json, "21.719", "Debe citar la ley.");
        }

        // Ley 21.719: bloqueo del tratamiento
        [TestMethod]
        public void InformePorCurso_OmiteEstudiantesConTratamientoBloqueado()
        {
            // Arrange: Ana y Bruno en POO; Bruno pidió bloquear el tratamiento
            var poo = new Asignatura { Id = 1, Codigo = "POO101", Nombre = "POO" };
            var ana = new Estudiante { Id = 10, Rut = "11111111-1", Nombres = "Ana", Apellidos = "Pérez" };
            var bruno = new Estudiante { Id = 20, Rut = "22222222-2", Nombres = "Bruno", Apellidos = "Soto", TratamientoBloqueado = true };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(ana, poo);
            fuente.Inscribir(bruno, poo);
            var servicio = new ServicioInformes(fuente);

            // Act
            var lineas = servicio.InformePorCurso(poo);

            // Assert
            Assert.AreEqual(1, lineas.Count, "El estudiante bloqueado no debe aparecer en el informe del curso.");
            Assert.AreEqual("11111111-1", lineas[0].Rut);
        }

        [TestMethod]
        public void EstudianteBloqueado_NoApareceEnListasNiInformesIndividuales()
        {
            var poo = new Asignatura { Id = 1, Codigo = "POO101", Nombre = "POO" };
            var bruno = new Estudiante { Id = 20, Rut = "22222222-2", Nombres = "Bruno", Apellidos = "Soto", TratamientoBloqueado = true };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(bruno, poo);
            var servicio = new ServicioInformes(fuente);

            Assert.IsFalse(bruno.PuedeTratarse());
            Assert.AreEqual(0, servicio.ObtenerEstudiantesDe(new List<Asignatura> { poo }).Count, "No debe aparecer en la lista del docente.");
            Assert.AreEqual(0, servicio.InformePorEstudiante(bruno).Count, "Sin informe individual (vista del administrador).");
            Assert.AreEqual(0, servicio.InformePorEstudiante(bruno, new List<Asignatura> { poo }).Count, "Sin informe individual (vista del docente).");
        }

        [TestMethod]
        public void DatosTitular_DeEstudianteBloqueado_SigueDisponible()
        {
            // El bloqueo no suspende el derecho de acceso: sus datos se pueden exportar completos
            var poo = new Asignatura { Id = 1, Codigo = "POO101", Nombre = "POO" };
            var bruno = new Estudiante { Id = 20, Rut = "22222222-2", Nombres = "Bruno", Apellidos = "Soto", TratamientoBloqueado = true };

            var fuente = new FuenteDatosConInscripciones();
            fuente.Inscribir(bruno, poo);
            fuente.AgregarNota(bruno, poo, new Calificacion { Nota = 5.0m, Evaluacion = new Evaluacion { Nombre = "Prueba 1", Ponderacion = 40m } });

            var datos = new ServicioDatosTitular(fuente).Generar(bruno, DateTime.UtcNow);

            Assert.IsTrue(datos.Titular.TratamientoBloqueado, "La exportación debe informar que el tratamiento está bloqueado.");
            Assert.AreEqual(1, datos.Asignaturas.Count, "Su historial se conserva.");
            Assert.AreEqual(5.0m, datos.Asignaturas[0].Notas[0].Nota);
        }

        // RF-07: Editar y eliminar evaluaciones
        [TestMethod]
        public void ActualizarEvaluacion_DentroDelLimite_SeAplica()
        {
            var asignatura = new Asignatura();
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 1, Nombre = "Prueba 1", Ponderacion = 60m });
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 2, Nombre = "Prueba 2", Ponderacion = 40m });

            asignatura.ActualizarEvaluacion(2, "Prueba 2 (corregida)", new DateTime(2026, 11, 20), 30m);

            var editada = asignatura.Evaluaciones.Find(e => e.Id == 2)!;
            Assert.AreEqual("Prueba 2 (corregida)", editada.Nombre);
            Assert.AreEqual(30m, editada.Ponderacion);
            Assert.AreEqual(90m, asignatura.SumaPonderaciones());
        }

        [TestMethod]
        public void ActualizarEvaluacion_QueSuperaCienPorCiento_EsRechazadaSinCambios()
        {
            var asignatura = new Asignatura();
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 1, Nombre = "Prueba 1", Ponderacion = 60m });
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 2, Nombre = "Prueba 2", Ponderacion = 40m });

            bool lanzoExcepcion = false;
            try
            {
                asignatura.ActualizarEvaluacion(2, "Prueba 2", DateTime.Today, 50m); // 60 + 50 = 110 %
            }
            catch (InvalidOperationException)
            {
                lanzoExcepcion = true;
            }

            Assert.IsTrue(lanzoExcepcion, "No puede superar el 100 %.");
            Assert.AreEqual(40m, asignatura.Evaluaciones.Find(e => e.Id == 2)!.Ponderacion, "La evaluación no debe cambiar.");
            Assert.AreEqual(100m, asignatura.SumaPonderaciones());
        }

        [TestMethod]
        public void QuitarEvaluacion_LiberaSuPonderacion()
        {
            var asignatura = new Asignatura();
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 1, Nombre = "Prueba 1", Ponderacion = 60m });
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 2, Nombre = "Prueba 2", Ponderacion = 40m });

            asignatura.QuitarEvaluacion(1);
            asignatura.AgregarEvaluacion(new Evaluacion { Id = 3, Nombre = "Examen", Ponderacion = 60m });

            Assert.AreEqual(2, asignatura.Evaluaciones.Count);
            Assert.AreEqual(100m, asignatura.SumaPonderaciones(), "Al quitar una evaluación, su ponderación vuelve a estar disponible.");
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

    // Fake con inscripciones configurables, para probar el control de acceso sin BD
    public class FuenteDatosConInscripciones : IFuenteDatosInformes
    {
        private readonly Dictionary<int, List<Asignatura>> asignaturasPorEstudiante = new Dictionary<int, List<Asignatura>>();
        private readonly Dictionary<int, Estudiante> estudiantes = new Dictionary<int, Estudiante>();

        public void Inscribir(Estudiante estudiante, params Asignatura[] asignaturas)
        {
            estudiantes[estudiante.Id] = estudiante;
            if (!asignaturasPorEstudiante.ContainsKey(estudiante.Id))
                asignaturasPorEstudiante[estudiante.Id] = new List<Asignatura>();

            asignaturasPorEstudiante[estudiante.Id].AddRange(asignaturas);
        }

        public List<Asignatura> ObtenerAsignaturasDeEstudiante(int estudianteId)
        {
            return asignaturasPorEstudiante.ContainsKey(estudianteId)
                ? new List<Asignatura>(asignaturasPorEstudiante[estudianteId])
                : new List<Asignatura>();
        }

        public List<Estudiante> ObtenerInscritos(int asignaturaId)
        {
            List<Estudiante> inscritos = new List<Estudiante>();
            foreach (KeyValuePair<int, List<Asignatura>> par in asignaturasPorEstudiante)
            {
                if (par.Value.Exists(a => a.Id == asignaturaId))
                    inscritos.Add(estudiantes[par.Key]);
            }
            return inscritos;
        }

        // Notas y asistencia por (estudiante, asignatura); vacías si no se agregan
        private readonly Dictionary<(int, int), List<Calificacion>> notas = new Dictionary<(int, int), List<Calificacion>>();
        private readonly Dictionary<(int, int), List<Asistencia>> asistencias = new Dictionary<(int, int), List<Asistencia>>();

        public void AgregarNota(Estudiante estudiante, Asignatura asignatura, Calificacion calificacion)
        {
            var clave = (estudiante.Id, asignatura.Id);
            if (!notas.ContainsKey(clave))
                notas[clave] = new List<Calificacion>();
            notas[clave].Add(calificacion);
        }

        public void AgregarAsistencia(Estudiante estudiante, Asignatura asignatura, Asistencia asistencia)
        {
            var clave = (estudiante.Id, asignatura.Id);
            if (!asistencias.ContainsKey(clave))
                asistencias[clave] = new List<Asistencia>();
            asistencias[clave].Add(asistencia);
        }

        public List<Calificacion> ObtenerCalificaciones(int estudianteId, int asignaturaId)
        {
            return notas.ContainsKey((estudianteId, asignaturaId))
                ? new List<Calificacion>(notas[(estudianteId, asignaturaId)])
                : new List<Calificacion>();
        }

        public List<Asistencia> ObtenerAsistencias(int estudianteId, int asignaturaId)
        {
            return asistencias.ContainsKey((estudianteId, asignaturaId))
                ? new List<Asistencia>(asistencias[(estudianteId, asignaturaId)])
                : new List<Asistencia>();
        }
    }
}