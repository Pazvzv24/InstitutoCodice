/* =========================================================
   Instituto Códice - Actualización de seguridad (v1.3)
   Agrega el bloqueo de cuenta por intentos fallidos:
     - IntentosFallidos: intentos fallidos consecutivos
     - BloqueadoHasta:   fecha y hora (UTC) de fin del bloqueo

   Solo para bases instaladas ANTES de la v1.3.
   Una instalación nueva con 01 + 02 ya incluye estas columnas.
   Se puede ejecutar más de una vez sin errores.
   ========================================================= */
USE InstitutoCodiceDB;
GO

IF COL_LENGTH('dbo.Usuario', 'IntentosFallidos') IS NULL
    ALTER TABLE [dbo].[Usuario] ADD [IntentosFallidos] [int] NOT NULL
        CONSTRAINT [DF_Usuario_IntentosFallidos] DEFAULT ((0));
GO

IF COL_LENGTH('dbo.Usuario', 'BloqueadoHasta') IS NULL
    ALTER TABLE [dbo].[Usuario] ADD [BloqueadoHasta] [datetime2](0) NULL;
GO

IF OBJECT_ID('CK_Usuario_IntentosFallidos', 'C') IS NULL
    ALTER TABLE [dbo].[Usuario] WITH CHECK ADD CONSTRAINT [CK_Usuario_IntentosFallidos]
        CHECK ([IntentosFallidos] >= 0);
GO

SELECT NombreUsuario, IntentosFallidos, BloqueadoHasta FROM Usuario;
GO

/* ---------------------------------------------------------
   Para desbloquear una cuenta manualmente (ejemplo):

   UPDATE Usuario SET IntentosFallidos = 0, BloqueadoHasta = NULL
   WHERE NombreUsuario = 'admin';
   --------------------------------------------------------- */

      UPDATE Usuario SET IntentosFallidos = 0, BloqueadoHasta = NULL
   WHERE NombreUsuario = 'pperez';

   USE InstitutoCodiceDB;
SELECT NombreUsuario, Activo, IntentosFallidos, BloqueadoHasta, LEFT(ContrasenaHash, 14) AS Formato
FROM Usuario;

UPDATE Usuario
SET Activo = 1, IntentosFallidos = 0, BloqueadoHasta = NULL
WHERE NombreUsuario = 'pperez';