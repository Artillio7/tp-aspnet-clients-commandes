IF DB_ID(N'TpClientsCommandesDb') IS NULL
BEGIN
    PRINT 'Creating database TpClientsCommandesDb';
    CREATE DATABASE [TpClientsCommandesDb];
END
GO

USE [TpClientsCommandesDb];
GO

IF OBJECT_ID(N'dbo.Clients', N'U') IS NULL
BEGIN
    PRINT 'Creating table dbo.Clients';
    CREATE TABLE dbo.Clients (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nom NVARCHAR(100) NOT NULL,
        Prenom NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        Telephone NVARCHAR(30) NOT NULL,
        Adresse NVARCHAR(300) NOT NULL,
        DateCreation DATETIME2 NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Commandes', N'U') IS NULL
BEGIN
    PRINT 'Creating table dbo.Commandes';
    CREATE TABLE dbo.Commandes (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NumeroCommande NVARCHAR(50) NOT NULL,
        DateCommande DATETIME2 NOT NULL,
        MontantTotal DECIMAL(18,2) NOT NULL,
        Statut NVARCHAR(50) NOT NULL,
        ClientId INT NOT NULL
    );

    PRINT 'Creating index IX_Commandes_ClientId';
    CREATE INDEX IX_Commandes_ClientId ON dbo.Commandes(ClientId);

    PRINT 'Adding FK FK_Commandes_Clients_ClientId ON DELETE CASCADE';
    ALTER TABLE dbo.Commandes
    ADD CONSTRAINT FK_Commandes_Clients_ClientId
        FOREIGN KEY (ClientId) REFERENCES dbo.Clients(Id) ON DELETE CASCADE;
END
GO