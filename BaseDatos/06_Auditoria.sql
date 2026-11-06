/* =========================================================
   Instituto Códice - Bitácora de auditoría (v1.5)
   Registra quién hizo qué y cuándo: ingresos, bloqueos,
   cambios de contraseña, notas y desactivaciones.

   - La tabla solo admite INSERT: un trigger impide modificar
     o borrar registros (bitácora inmutable).
   - Nunca se guardan contraseñas.

   Solo para bases instaladas ANTES de la v1.5.
   Se puede ejecutar más de una vez sin errores.
   ========================================================= */
USE InstitutoCodiceDB;
GO

IF OBJECT_ID('dbo.Auditoria', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Auditoria](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [FechaUtc] [datetime2](0) NOT NULL CONSTRAINT [DF_Auditoria_FechaUtc] DEFAULT (SYSUTCDATETIME()),
        [UsuarioId] [int] NULL,                -- NULL si el usuario no existe (login con nombre desconocido)
        [NombreUsuario] [nvarchar](50) NOT NULL,
        [Accion] [nvarchar](40) NOT NULL,
        [Detalle] [nvarchar](500) NULL,
        [Entidad] [nvarchar](50) NULL,         -- Tabla afectada (Calificacion, Usuario, Estudiante...)
        [EntidadId] [int] NULL,
        [ValorAnterior] [nvarchar](100) NULL,
        [ValorNuevo] [nvarchar](100) NULL,
        CONSTRAINT [PK_Auditoria] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Auditoria_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id])
    );

    CREATE NONCLUSTERED INDEX [IX_Auditoria_FechaUtc] ON [dbo].[Auditoria] ([FechaUtc] DESC);
END
GO

-- Bitácora inmutable: rechaza cualquier UPDATE o DELETE
CREATE OR ALTER TRIGGER [dbo].[TR_Auditoria_SoloInsercion]
ON [dbo].[Auditoria]
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    THROW 50001, 'La bitácora de auditoría no se puede modificar ni eliminar.', 1;
END
GO

SELECT COUNT(*) AS RegistrosAuditoria FROM Auditoria;
GO