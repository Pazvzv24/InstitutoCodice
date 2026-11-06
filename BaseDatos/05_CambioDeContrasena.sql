/* =========================================================
   Instituto Códice - Actualización de seguridad (v1.4)
   Agrega el cambio de contraseña obligatorio:
     - DebeCambiarContrasena: 1 = el usuario debe cambiarla al ingresar
       (se activa cuando el administrador crea o restablece una contraseña).

   Las cuentas existentes quedan en 0 (no se les exige el cambio).
   Solo para bases instaladas ANTES de la v1.4.
   Se puede ejecutar más de una vez sin errores.
   ========================================================= */
USE InstitutoCodiceDB;
GO

IF COL_LENGTH('dbo.Usuario', 'DebeCambiarContrasena') IS NULL
    ALTER TABLE [dbo].[Usuario] ADD [DebeCambiarContrasena] [bit] NOT NULL
        CONSTRAINT [DF_Usuario_DebeCambiarContrasena] DEFAULT ((0));
GO

SELECT NombreUsuario, Activo, DebeCambiarContrasena FROM Usuario;
GO

/* ---------------------------------------------------------
   Para obligar a un usuario a cambiar su contraseña (ejemplo):

   UPDATE Usuario SET DebeCambiarContrasena = 1
   WHERE NombreUsuario = 'pperez';
   --------------------------------------------------------- */