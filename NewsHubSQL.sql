IF DB_ID('PAW_NewsHub') IS NULL
    CREATE DATABASE PAW_NewsHub;
GO

USE PAW_NewsHub;
GO

-- Limpieza previa
IF OBJECT_ID('dbo.Secrets', 'U')       IS NOT NULL DROP TABLE dbo.Secrets;
IF OBJECT_ID('dbo.SourceItems', 'U')   IS NOT NULL DROP TABLE dbo.SourceItems;
IF OBJECT_ID('dbo.Sources', 'U')       IS NOT NULL DROP TABLE dbo.Sources;
GO

-- TABLAS PROFE 
CREATE TABLE dbo.Sources (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Url             NVARCHAR(500)  NOT NULL,
    Name            NVARCHAR(200)  NOT NULL,
    Description     NVARCHAR(500)  NULL,
    ComponentType   NVARCHAR(100)  NOT NULL,
    RequiresSecret  BIT            NOT NULL 
        CONSTRAINT DF_Sources_RequiresSecret DEFAULT (0)
);
GO

CREATE TABLE dbo.SourceItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    SourceId    INT              NOT NULL,
    Json        NVARCHAR(MAX)    NOT NULL,
    CreatedAt   DATETIME         NOT NULL 
        CONSTRAINT DF_SourceItems_CreatedAt DEFAULT (GETUTCDATE())
);
GO

ALTER TABLE dbo.SourceItems
ADD CONSTRAINT FK_SourceItems_Sources
    FOREIGN KEY (SourceId) REFERENCES dbo.Sources(Id);
GO

-- Index
CREATE NONCLUSTERED INDEX IX_Sources_Name
    ON dbo.Sources (Name);

CREATE NONCLUSTERED INDEX IX_SourceItems_SourceId
    ON dbo.SourceItems (SourceId);
GO

-- Secrets (Adicional)
CREATE TABLE dbo.Secrets (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)   NOT NULL,
    Value           NVARCHAR(4000)  NOT NULL,
    Description     NVARCHAR(500)   NULL,
    SourceId        INT             NULL,
    IsActive        BIT             NOT NULL 
        CONSTRAINT DF_Secrets_IsActive DEFAULT (1),
    CreatedAt       DATETIME        NOT NULL 
        CONSTRAINT DF_Secrets_CreatedAt DEFAULT (GETUTCDATE())
);
GO

ALTER TABLE dbo.Secrets
ADD CONSTRAINT FK_Secrets_Sources
    FOREIGN KEY (SourceId) REFERENCES dbo.Sources(Id);

ALTER TABLE dbo.Secrets
ADD CONSTRAINT UQ_Secrets_Name UNIQUE (Name);
GO

--Tablas Usuarios

CREATE TABLE Roles (
    RoleID      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(255) NOT NULL
);

CREATE TABLE Users (
    UserID        INT IDENTITY(1,1) PRIMARY KEY,
    Username      NVARCHAR(255) NOT NULL,
    Email         NVARCHAR(255) NULL,
    PasswordHash  NVARCHAR(255) NOT NULL,
    CreatedAt     DATETIME      NULL DEFAULT(GETDATE()),
    IsActive      BIT           NOT NULL DEFAULT(1),
    LastModified  DATETIME      NULL,
    ModifiedBy    NVARCHAR(255) NULL,
    RoleID        INT           NULL
        CONSTRAINT FK_Users_Roles
        REFERENCES Roles(RoleID)
);

CREATE TABLE UserRoles (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    RoleID  INT NOT NULL
        CONSTRAINT FK_UserRoles_Roles
        REFERENCES Roles(RoleID),
    UserID  INT NOT NULL
        CONSTRAINT FK_UserRoles_Users
        REFERENCES Users(UserID)
);
