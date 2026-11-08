/* =========================================================
   Instituto Códice - Permisos de eliminación (v2.0)

   Desde la v2.0 la aplicación permite:
   - Eliminar una nota mal ingresada (queda en la bitácora con su valor anterior).
   - Eliminar una evaluación que todavía no tiene notas.

   Por mínimo privilegio, el rol de la aplicación solo tenía permiso para borrar
   inscripciones. Este script le da permiso de DELETE ÚNICAMENTE sobre las tablas
   Calificacion y Evaluacion. La bitácora (Auditoria) sigue con DELETE denegado.

   Solo para bases instaladas ANTES de la v2.0.
   Se puede ejecutar más de una vez sin errores.
   Ejecutar con una cuenta administradora (no con app_codice).
   ========================================================= */
USE InstitutoCodiceDB;
GO

IF DATABASE_PRINCIPAL_ID(N'rol_app_codice') IS NULL
BEGIN
    PRINT 'El rol rol_app_codice no existe: primero ejecute 07_Rol_App_Codice.sql.';
END
ELSE
BEGIN
    GRANT DELETE ON dbo.Calificacion TO rol_app_codice;
    GRANT DELETE ON dbo.Evaluacion   TO rol_app_codice;
    PRINT 'Permisos de eliminación otorgados sobre Calificacion y Evaluacion.';
END
GO

-- Verificación: permisos de DELETE del rol (Auditoria debe aparecer como DENY)
SELECT OBJECT_NAME(p.major_id) AS Tabla, p.permission_name AS Permiso, p.state_desc AS Estado
FROM sys.database_permissions p
WHERE p.grantee_principal_id = DATABASE_PRINCIPAL_ID(N'rol_app_codice')
  AND p.permission_name = 'DELETE'
ORDER BY Tabla;
GO