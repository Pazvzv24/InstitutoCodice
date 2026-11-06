/* =========================================================
   Instituto Códice - Actualización de seguridad
   Reemplaza los hashes SHA-256 antiguos de las cuentas demo
   por hashes PBKDF2-SHA256 (600.000 iteraciones + sal).
   ========================================================= */
   USE InstitutoCodiceDB;
GO

UPDATE Usuario SET ContrasenaHash = 'PBKDF2$600000$eUyStY5P7KUSVtnx9asObw==$diY0Gn0dUBFIMTE3sHUJDBAhqqQ3MDD6rRI+wx7SqS0='
WHERE NombreUsuario = 'admin';

UPDATE Usuario SET ContrasenaHash = 'PBKDF2$600000$SIrBImBsYKvg0wQ6tcrlug==$kvJDFXbnp6hj1+QU0+17iOCW1J298U08g6aehDX4c0k='
WHERE NombreUsuario = 'pmunoz';

UPDATE Usuario SET ContrasenaHash = 'PBKDF2$600000$gTK4hmnD1Z9rpWuBA4/EAw==$yr4PxQWJB7qdrknPhzE7Z+tfeXCV0eWuo4euA/V7eKU='
WHERE NombreUsuario = 'cvaldes';

UPDATE Usuario SET ContrasenaHash = 'PBKDF2$600000$LmTIgz3bcUG41eo+CDEY3w==$aszZCMCTVKRiLnK2RfGGk1iyfBjiCRvB/DMhZTaU4js='
WHERE NombreUsuario = 'pperez';

SELECT NombreUsuario, LEFT(ContrasenaHash, 14) AS Formato FROM Usuario;
GO

IF OBJECT_ID('CK_Usuario_HashFormato', 'C') IS NULL
    ALTER TABLE [dbo].[Usuario] WITH CHECK ADD CONSTRAINT [CK_Usuario_HashFormato]
        CHECK ([ContrasenaHash] LIKE 'PBKDF2$%$%$%');
GO


