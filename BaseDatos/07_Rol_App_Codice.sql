/* =========================================================
   Instituto Códice - Mínimo privilegio (v1.6)
   Crea el rol de la aplicación con SOLO los permisos que necesita
   y asigna el usuario app_codice a ese rol.

   Requisitos previos (lo hace un administrador, una vez):
     1. Servidor en modo de autenticación "Windows y SQL Server".
     2. Login creado:
          CREATE LOGIN app_codice WITH PASSWORD = N'...',
                DEFAULT_DATABASE = InstitutoCodiceDB, CHECK_POLICY = ON;
        (La contraseña NUNCA se guarda en este archivo ni en el repositorio.)

   Se puede ejecutar más de una vez sin errores.
   ========================================================= */
USE InstitutoCodiceDB;
GO

-- 1. Rol de la aplicación
IF DATABASE_PRINCIPAL_ID('rol_app_codice') IS NULL
    CREATE ROLE rol_app_codice;
GO

-- 2. Permisos mínimos
-- Leer, insertar y actualizar en las tablas de la app
GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO rol_app_codice;

-- Borrar SOLO donde la app lo necesita (anular una inscripción)
GRANT DELETE ON dbo.Inscripcion TO rol_app_codice;

-- La bitácora solo admite inserción (refuerza el trigger)
DENY UPDATE, DELETE ON dbo.Auditoria TO rol_app_codice;

-- Sin permisos de ALTER, CREATE, DROP ni CONTROL:
-- la app no puede crear o borrar tablas, ni desactivar el trigger de auditoría.
GO

-- 3. Usuario de la app dentro de la base, asociado al login
IF SUSER_ID('app_codice') IS NOT NULL AND DATABASE_PRINCIPAL_ID('app_codice') IS NULL
    CREATE USER app_codice FOR LOGIN app_codice;
GO

IF DATABASE_PRINCIPAL_ID('app_codice') IS NOT NULL
    ALTER ROLE rol_app_codice ADD MEMBER app_codice;
GO

-- 4. Verificación
SELECT r.name AS Rol, m.name AS Miembro
FROM sys.database_role_members rm
JOIN sys.database_principals r ON r.principal_id = rm.role_principal_id
JOIN sys.database_principals m ON m.principal_id = rm.member_principal_id
WHERE r.name = 'rol_app_codice';
GO