/* =========================================================
   Datos de prueba - InstitutoCodiceDB
   Ejecutar después de 01_InstitutoCodiceDB.sql, una sola vez.

   Usuarios:
     admin   / Admin123     (Administrador)
     pmunoz  / Docente123   (Docente - POO101, TPR205)
     cvaldes / Docente123   (Docente - BDD201)

   Claves con hash PBKDF2-SHA256, 600.000 iteraciones + sal (RNF-03).
   Credenciales ficticias de demostración, solo para entorno local.
   ========================================================= */

USE InstitutoCodiceDB;
GO
SET NOCOUNT ON;

-- 0. Estados de asistencia (catálogo requerido por Asistencia.EstadoId)
INSERT INTO EstadoAsistencia (Id, Nombre) VALUES
(1, N'Presente'),
(2, N'Ausente'),
(3, N'Justificado');

-- 1. Usuarios
INSERT INTO Usuario (NombreUsuario, ContrasenaHash, Nombre, Rol, Activo) VALUES
('admin', 'PBKDF2$600000$eUyStY5P7KUSVtnx9asObw==$diY0Gn0dUBFIMTE3sHUJDBAhqqQ3MDD6rRI+wx7SqS0=', N'Administrador Académico', 'Administrador', 1),
('pmunoz', 'PBKDF2$600000$SIrBImBsYKvg0wQ6tcrlug==$kvJDFXbnp6hj1+QU0+17iOCW1J298U08g6aehDX4c0k=', N'Pedro Muñoz Rojas', 'Docente', 1),
('cvaldes', 'PBKDF2$600000$gTK4hmnD1Z9rpWuBA4/EAw==$yr4PxQWJB7qdrknPhzE7Z+tfeXCV0eWuo4euA/V7eKU=', N'Carolina Valdés Soto', 'Docente', 1);

-- 2. Docentes
INSERT INTO Docente (Id, Rut, Correo)
SELECT Id, '13456789-9', 'pedro.munoz@codice.cl' FROM Usuario WHERE NombreUsuario = 'pmunoz'
UNION ALL
SELECT Id, '14567890-0', 'carolina.valdes@codice.cl' FROM Usuario WHERE NombreUsuario = 'cvaldes';

-- 3. Estudiantes
INSERT INTO Estudiante (Rut, Nombres, Apellidos, Correo, Curso, Activo) VALUES
('20123456-5', N'Camila', N'González Pérez', 'camila.gonzalez@alumnos.codice.cl', 'TI-301', 1),
('20234567-0', N'Matías', N'Rojas Fuentes', 'matias.rojas@alumnos.codice.cl', 'TI-301', 1),
('20345678-6', N'Valentina', N'Muñoz Castro', 'valentina.munoz@alumnos.codice.cl', 'TI-301', 1),
('20456789-1', N'Benjamín', N'Díaz Herrera', NULL, 'TI-301', 1),
('20567890-5', N'Fernanda', N'Soto Morales', 'fernanda.soto@alumnos.codice.cl', 'TI-301', 1),
('20678901-8', N'Tomás', N'Contreras Silva', 'tomas.contreras@alumnos.codice.cl', 'TI-301', 1),
('21012345-8', N'Antonia', N'Reyes Vargas', 'antonia.reyes@alumnos.codice.cl', 'TI-302', 1),
('21123456-3', N'Diego', N'Espinoza Torres', 'diego.espinoza@alumnos.codice.cl', 'TI-302', 1);

-- 4. Asignaturas
INSERT INTO Asignatura (Codigo, Nombre, Curso, DocenteId)
SELECT v.Codigo, v.Nombre, v.Curso, u.Id
FROM (VALUES
    ('POO101', N'Programación Orientada a Objetos', 'TI-301', 'pmunoz'),
    ('BDD201', N'Bases de Datos', 'TI-301', 'cvaldes'),
    ('TPR205', N'Taller de Programación', 'TI-302', 'pmunoz')
) AS v(Codigo, Nombre, Curso, NombreUsuario)
JOIN Usuario u ON u.NombreUsuario = v.NombreUsuario;

-- 5. Inscripciones
INSERT INTO Inscripcion (EstudianteId, AsignaturaId, FechaInscripcion)
SELECT e.Id, a.Id, v.Fecha
FROM (VALUES
    ('20123456-5', 'POO101', '2026-08-17'),
    ('20234567-0', 'POO101', '2026-08-17'),
    ('20345678-6', 'POO101', '2026-08-17'),
    ('20456789-1', 'POO101', '2026-08-17'),
    ('20567890-5', 'POO101', '2026-08-17'),
    ('20678901-8', 'POO101', '2026-08-17'),
    ('20123456-5', 'BDD201', '2026-08-17'),
    ('20234567-0', 'BDD201', '2026-08-17'),
    ('20345678-6', 'BDD201', '2026-08-17'),
    ('20456789-1', 'BDD201', '2026-08-17'),
    ('20567890-5', 'BDD201', '2026-08-17'),
    ('20678901-8', 'BDD201', '2026-08-17'),
    ('21012345-8', 'TPR205', '2026-08-17'),
    ('21123456-3', 'TPR205', '2026-08-17')
) AS v(Rut, Codigo, Fecha)
JOIN Estudiante e ON e.Rut = v.Rut
JOIN Asignatura a ON a.Codigo = v.Codigo;

-- 6. Evaluaciones
INSERT INTO Evaluacion (AsignaturaId, Nombre, Fecha, Ponderacion)
SELECT a.Id, v.Nombre, v.Fecha, v.Ponderacion
FROM (VALUES
    ('POO101', N'Prueba 1', '2026-08-28', 30),
    ('POO101', N'Prueba 2', '2026-09-25', 30),
    ('POO101', N'Proyecto final', '2026-10-09', 40),
    ('BDD201', N'Prueba 1', '2026-09-04', 50),
    ('BDD201', N'Trabajo práctico', '2026-10-30', 50),
    ('TPR205', N'Taller 1', '2026-09-11', 40),
    ('TPR205', N'Taller 2', '2026-11-06', 60)
) AS v(Codigo, Nombre, Fecha, Ponderacion)
JOIN Asignatura a ON a.Codigo = v.Codigo;

-- 7. Calificaciones
INSERT INTO Calificacion (EstudianteId, EvaluacionId, Nota)
SELECT e.Id, ev.Id, v.Nota
FROM (VALUES
    ('20123456-5', 'POO101', N'Prueba 1', 6.0),
    ('20234567-0', 'POO101', N'Prueba 1', 4.5),
    ('20345678-6', 'POO101', N'Prueba 1', 5.8),
    ('20456789-1', 'POO101', N'Prueba 1', 3.2),
    ('20567890-5', 'POO101', N'Prueba 1', 6.5),
    ('20678901-8', 'POO101', N'Prueba 1', 4.0),
    ('20123456-5', 'POO101', N'Prueba 2', 6.3),
    ('20234567-0', 'POO101', N'Prueba 2', 3.8),
    ('20345678-6', 'POO101', N'Prueba 2', 6.1),
    ('20456789-1', 'POO101', N'Prueba 2', 3.5),
    ('20567890-5', 'POO101', N'Prueba 2', 6.8),
    ('20678901-8', 'POO101', N'Prueba 2', 4.2),
    ('20123456-5', 'POO101', N'Proyecto final', 6.5),
    ('20234567-0', 'POO101', N'Proyecto final', 4.4),
    ('20345678-6', 'POO101', N'Proyecto final', 5.5),
    ('20456789-1', 'POO101', N'Proyecto final', 3.9),
    ('20567890-5', 'POO101', N'Proyecto final', 7.0),
    ('20678901-8', 'POO101', N'Proyecto final', 3.6),
    ('20123456-5', 'BDD201', N'Prueba 1', 5.5),
    ('20234567-0', 'BDD201', N'Prueba 1', 4.1),
    ('20345678-6', 'BDD201', N'Prueba 1', 6.7),
    ('20456789-1', 'BDD201', N'Prueba 1', 2.8),
    ('20567890-5', 'BDD201', N'Prueba 1', 6.0),
    ('20678901-8', 'BDD201', N'Prueba 1', 5.0),
    ('21012345-8', 'TPR205', N'Taller 1', 5.2),
    ('21123456-3', 'TPR205', N'Taller 1', 6.4)
) AS v(Rut, Codigo, NombreEval, Nota)
JOIN Estudiante e ON e.Rut = v.Rut
JOIN Asignatura a ON a.Codigo = v.Codigo
JOIN Evaluacion ev ON ev.AsignaturaId = a.Id AND ev.Nombre = v.NombreEval;

-- 8. Asistencia (1 Presente, 2 Ausente, 3 Justificado)
INSERT INTO Asistencia (EstudianteId, AsignaturaId, Fecha, EstadoId)
SELECT e.Id, a.Id, v.Fecha, v.EstadoId
FROM (VALUES
    -- POO101
    ('20123456-5', 'POO101', '2026-09-01', 1), ('20123456-5', 'POO101', '2026-09-08', 1), ('20123456-5', 'POO101', '2026-09-15', 1), ('20123456-5', 'POO101', '2026-09-22', 1), ('20123456-5', 'POO101', '2026-09-29', 1), ('20123456-5', 'POO101', '2026-10-06', 1),
    ('20234567-0', 'POO101', '2026-09-01', 1), ('20234567-0', 'POO101', '2026-09-08', 2), ('20234567-0', 'POO101', '2026-09-15', 1), ('20234567-0', 'POO101', '2026-09-22', 1), ('20234567-0', 'POO101', '2026-09-29', 3), ('20234567-0', 'POO101', '2026-10-06', 1),
    ('20345678-6', 'POO101', '2026-09-01', 1), ('20345678-6', 'POO101', '2026-09-08', 1), ('20345678-6', 'POO101', '2026-09-15', 1), ('20345678-6', 'POO101', '2026-09-22', 1), ('20345678-6', 'POO101', '2026-09-29', 1), ('20345678-6', 'POO101', '2026-10-06', 2),
    ('20456789-1', 'POO101', '2026-09-01', 2), ('20456789-1', 'POO101', '2026-09-08', 2), ('20456789-1', 'POO101', '2026-09-15', 1), ('20456789-1', 'POO101', '2026-09-22', 2), ('20456789-1', 'POO101', '2026-09-29', 1), ('20456789-1', 'POO101', '2026-10-06', 2),
    ('20567890-5', 'POO101', '2026-09-01', 1), ('20567890-5', 'POO101', '2026-09-08', 1), ('20567890-5', 'POO101', '2026-09-15', 1), ('20567890-5', 'POO101', '2026-09-22', 1), ('20567890-5', 'POO101', '2026-09-29', 1), ('20567890-5', 'POO101', '2026-10-06', 1),
    ('20678901-8', 'POO101', '2026-09-01', 1), ('20678901-8', 'POO101', '2026-09-08', 1), ('20678901-8', 'POO101', '2026-09-15', 2), ('20678901-8', 'POO101', '2026-09-22', 1), ('20678901-8', 'POO101', '2026-09-29', 1), ('20678901-8', 'POO101', '2026-10-06', 1),
    -- BDD201
    ('20123456-5', 'BDD201', '2026-09-03', 1), ('20123456-5', 'BDD201', '2026-09-10', 1), ('20123456-5', 'BDD201', '2026-09-17', 1), ('20123456-5', 'BDD201', '2026-09-24', 1),
    ('20234567-0', 'BDD201', '2026-09-03', 1), ('20234567-0', 'BDD201', '2026-09-10', 1), ('20234567-0', 'BDD201', '2026-09-17', 2), ('20234567-0', 'BDD201', '2026-09-24', 1),
    ('20345678-6', 'BDD201', '2026-09-03', 1), ('20345678-6', 'BDD201', '2026-09-10', 1), ('20345678-6', 'BDD201', '2026-09-17', 1), ('20345678-6', 'BDD201', '2026-09-24', 1),
    ('20456789-1', 'BDD201', '2026-09-03', 1), ('20456789-1', 'BDD201', '2026-09-10', 2), ('20456789-1', 'BDD201', '2026-09-17', 2), ('20456789-1', 'BDD201', '2026-09-24', 1),
    ('20567890-5', 'BDD201', '2026-09-03', 1), ('20567890-5', 'BDD201', '2026-09-10', 1), ('20567890-5', 'BDD201', '2026-09-17', 1), ('20567890-5', 'BDD201', '2026-09-24', 1),
    ('20678901-8', 'BDD201', '2026-09-03', 2), ('20678901-8', 'BDD201', '2026-09-10', 1), ('20678901-8', 'BDD201', '2026-09-17', 1), ('20678901-8', 'BDD201', '2026-09-24', 1)
) AS v(Rut, Codigo, Fecha, EstadoId)
JOIN Estudiante e ON e.Rut = v.Rut
JOIN Asignatura a ON a.Codigo = v.Codigo;

-- Conteo
SELECT 'EstadoAsistencia' AS Tabla, COUNT(*) AS Registros FROM EstadoAsistencia
UNION ALL SELECT 'Usuario', COUNT(*) FROM Usuario
UNION ALL SELECT 'Docente', COUNT(*) FROM Docente
UNION ALL SELECT 'Estudiante', COUNT(*) FROM Estudiante
UNION ALL SELECT 'Asignatura', COUNT(*) FROM Asignatura
UNION ALL SELECT 'Inscripcion', COUNT(*) FROM Inscripcion
UNION ALL SELECT 'Evaluacion', COUNT(*) FROM Evaluacion
UNION ALL SELECT 'Calificacion', COUNT(*) FROM Calificacion
UNION ALL SELECT 'Asistencia', COUNT(*) FROM Asistencia;
GO