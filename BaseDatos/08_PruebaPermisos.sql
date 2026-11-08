/* =========================================================
   Instituto Códice - Prueba de permisos de app_codice (v2.0)
   Actúa como la aplicación y comprueba qué puede y qué NO puede hacer.

   Es segura: los DELETE usan WHERE 1 = 0, así que nunca borran filas;
   solo comprueban si SQL Server concede o rechaza el permiso.

   Resultado esperado: todas las líneas con ✅.
   Nota: al intentar desactivar el trigger, SQL Server responde
   "Cannot find the object ... or you do not have permission".
   Es el bloqueo esperado: a quien no tiene permiso no le confirma
   ni siquiera que el objeto existe.
   Si aparece alguna línea con ERROR, revisar 07_Rol_App_Codice.sql
   (o 10_PermisosEliminacion.sql en instalaciones anteriores a la v2.0).
   ========================================================= */
USE InstitutoCodiceDB;
GO
EXECUTE AS USER = 'app_codice';   -- desde aquí actúas como la app

PRINT N'--- Lo que la aplicación SÍ puede hacer ---';

BEGIN TRY DECLARE @n int = (SELECT COUNT(*) FROM Estudiante); PRINT N'✅ Permitido: leer estudiantes.'; END TRY
BEGIN CATCH PRINT N'ERROR: no pudo leer estudiantes. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Inscripcion WHERE 1 = 0;        PRINT N'✅ Permitido: anular inscripciones.'; END TRY
BEGIN CATCH PRINT N'ERROR: no pudo borrar inscripciones. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Calificacion WHERE 1 = 0;       PRINT N'✅ Permitido: eliminar notas (quedan en la bitácora).'; END TRY
BEGIN CATCH PRINT N'ERROR: no pudo borrar notas. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Evaluacion WHERE 1 = 0;         PRINT N'✅ Permitido: eliminar evaluaciones sin notas.'; END TRY
BEGIN CATCH PRINT N'ERROR: no pudo borrar evaluaciones. ' + ERROR_MESSAGE(); END CATCH

PRINT N'';
PRINT N'--- Lo que la aplicación NO puede hacer ---';

BEGIN TRY DELETE FROM Auditoria WHERE 1 = 0;          PRINT N'ERROR: pudo borrar la auditoría'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: borrar la auditoría. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY UPDATE Auditoria SET Detalle = Detalle WHERE 1 = 0; PRINT N'ERROR: pudo modificar la auditoría'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: modificar la auditoría. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Estudiante WHERE 1 = 0;         PRINT N'ERROR: pudo borrar estudiantes'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: borrar estudiantes (se desactivan). ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Docente WHERE 1 = 0;            PRINT N'ERROR: pudo borrar docentes'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: borrar docentes (se desactivan). ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Usuario WHERE 1 = 0;            PRINT N'ERROR: pudo borrar usuarios'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: borrar usuarios. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DISABLE TRIGGER TR_Auditoria_SoloInsercion ON Auditoria; PRINT N'ERROR: pudo desactivar el trigger'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: desactivar el trigger. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY CREATE TABLE Prueba (Id int);               PRINT N'ERROR: pudo crear tablas'; END TRY
BEGIN CATCH PRINT N'✅ Bloqueado: crear tablas. ' + ERROR_MESSAGE(); END CATCH

REVERT;   -- vuelves a ser tú
GO