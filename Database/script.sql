-- Prueba tecnica - Gestion de Clientes
-- Base de datos: GestionClientes

IF DB_ID('GestionClientes') IS NULL
BEGIN
    CREATE DATABASE GestionClientes;
END
GO

USE GestionClientes;
GO

-- Si ya existen las tablas las borro, para poder correr el script varias veces
IF OBJECT_ID('dbo.Bitacora', 'U') IS NOT NULL DROP TABLE dbo.Bitacora;
IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO

-- Usuarios del sistema
CREATE TABLE dbo.Usuarios
(
    Id            INT IDENTITY(1,1) NOT NULL,
    NombreUsuario NVARCHAR(50)  NOT NULL,
    PasswordHash  NVARCHAR(200) NOT NULL,
    PasswordSalt  NVARCHAR(100) NOT NULL,
    Activo        BIT           NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    FechaCreacion DATETIME2     NOT NULL CONSTRAINT DF_Usuarios_Fecha DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario)
);
GO
 
-- Clientes
CREATE TABLE dbo.Clientes
(
    Id            INT IDENTITY(1,1) NOT NULL,
    Nombres       NVARCHAR(100) NOT NULL,
    Apellidos     NVARCHAR(100) NOT NULL,
    Documento     NVARCHAR(20)  NOT NULL,
    Correo        NVARCHAR(100) NULL,
    Telefono      NVARCHAR(20)  NULL,
    Direccion     NVARCHAR(200) NULL,
    FechaCreacion DATETIME2     NOT NULL CONSTRAINT DF_Clientes_Fecha DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Clientes PRIMARY KEY (Id),
    CONSTRAINT UQ_Clientes_Documento UNIQUE (Documento)
);
GO

-- Bitacora de cambios
-- No lleva llave foranea a Clientes para que el registro quede aunque el cliente se elimine
CREATE TABLE dbo.Bitacora
(
    Id            BIGINT IDENTITY(1,1) NOT NULL,
    Accion        NVARCHAR(20)  NOT NULL,
    ClienteId     INT           NOT NULL,
    NombreUsuario NVARCHAR(50)  NOT NULL,
    FechaHora     DATETIME2     NOT NULL CONSTRAINT DF_Bitacora_Fecha DEFAULT (SYSDATETIME()),
    Detalle       NVARCHAR(500) NULL,
    CONSTRAINT PK_Bitacora PRIMARY KEY (Id),
    CONSTRAINT CK_Bitacora_Accion CHECK (Accion IN ('AGREGAR', 'EDITAR', 'ELIMINAR'))
);
GO

CREATE INDEX IX_Bitacora_ClienteId ON dbo.Bitacora (ClienteId);
CREATE INDEX IX_Bitacora_FechaHora ON dbo.Bitacora (FechaHora DESC);
GO

-- Usuario inicial para poder entrar al sistema
-- El hash esta hecho con PBKDF2 (SHA256, 100000 iteraciones) y la sal es aleatoria
-- Usuario: admin / Contrasena: admin123
INSERT INTO dbo.Usuarios (NombreUsuario, PasswordHash, PasswordSalt, Activo)
VALUES ('admin',
        'B63639224CD022A76A722FB8E3B25C29D2E24DA1CB8C1B73510247508B95FBE7',
        '607269A1BC8B827DE97B02E564FB0D7F',
        1);
GO

PRINT 'Base de datos GestionClientes creada correctamente.';
GO
