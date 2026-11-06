/* =========================================================
   Instituto Códice - Bloqueo del tratamiento (v1.9)
   Ley 21.719: el titular puede pedir que sus datos se
   "congelen" temporalmente. Un estudiante con el tratamiento
   bloqueado conserva todo su historial, pero no aparece en
   inscripciones, asistencia, calificaciones ni informes.

   Solo para bases instaladas ANTES de la v1.9.
   Se puede ejecutar más de una vez sin errores.
   Ejecutar con una cuenta administradora (no con app_codice).
   ========================================================= */
USE InstitutoCodiceDB;
GO

IF COL_LENGTH('dbo.Estudiante', 'TratamientoBloqueado') IS NULL
BEGIN
    ALTER TABLE [dbo].[Estudiante]
        ADD [TratamientoBloqueado] [bit] NOT NULL
            CONSTRAINT [DF_Estudiante_TratamientoBloqueado] DEFAULT ((0));
    PRINT 'Columna TratamientoBloqueado agregada.';
END
ELSE
    PRINT 'La columna TratamientoBloqueado ya existía.';
GO

-- Verificación: todos los estudiantes deben quedar con el tratamiento habilitado (0)
SELECT Rut, Nombres, Apellidos, Activo, TratamientoBloqueado
FROM dbo.Estudiante
ORDER BY Apellidos, Nombres;
GO