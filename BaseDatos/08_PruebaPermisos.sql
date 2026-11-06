USE InstitutoCodiceDB;
GO
EXECUTE AS USER = 'app_codice';   -- desde aquí actúas como la app

SELECT COUNT(*) AS PuedeLeer FROM Estudiante;          -- ✅ debe funcionar

BEGIN TRY DELETE FROM Auditoria;                      PRINT 'ERROR: pudo borrar la auditoría'; END TRY
BEGIN CATCH PRINT '✅ Bloqueado: borrar la auditoría. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DELETE FROM Calificacion WHERE 1 = 0;       PRINT 'ERROR: pudo borrar notas'; END TRY
BEGIN CATCH PRINT '✅ Bloqueado: borrar notas. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY DISABLE TRIGGER TR_Auditoria_SoloInsercion ON Auditoria; PRINT 'ERROR: pudo desactivar el trigger'; END TRY
BEGIN CATCH PRINT '✅ Bloqueado: desactivar el trigger. ' + ERROR_MESSAGE(); END CATCH

BEGIN TRY CREATE TABLE Prueba (Id int);               PRINT 'ERROR: pudo crear tablas'; END TRY
BEGIN CATCH PRINT '✅ Bloqueado: crear tablas. ' + ERROR_MESSAGE(); END CATCH

REVERT;   -- vuelves a ser tú
GO