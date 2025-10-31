USE [TpClientsCommandesDb];

SET NOCOUNT ON;

-- Clients (upsert by Email)
IF NOT EXISTS (SELECT 1 FROM dbo.Clients WHERE Email='jane.doe@example.com')
BEGIN
    INSERT INTO dbo.Clients (Nom, Prenom, Email, Telephone, Adresse, DateCreation)
    VALUES ('Doe','Jane','jane.doe@example.com','0102030405','12 Rue des Fleurs, Paris', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Clients WHERE Email='john.smith@example.com')
BEGIN
    INSERT INTO dbo.Clients (Nom, Prenom, Email, Telephone, Adresse, DateCreation)
    VALUES ('Smith','John','john.smith@example.com','0607080910','34 Avenue Victor Hugo, Lyon', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Clients WHERE Email='alice.martin@example.com')
BEGIN
    INSERT INTO dbo.Clients (Nom, Prenom, Email, Telephone, Adresse, DateCreation)
    VALUES ('Martin','Alice','alice.martin@example.com','0499123456','5 Place Bellecour, Lyon', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Clients WHERE Email='bob.durand@example.com')
BEGIN
    INSERT INTO dbo.Clients (Nom, Prenom, Email, Telephone, Adresse, DateCreation)
    VALUES ('Durand','Bob','bob.durand@example.com','0333123456','8 Rue Nationale, Lille', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Clients WHERE Email='clara.lefevre@example.com')
BEGIN
    INSERT INTO dbo.Clients (Nom, Prenom, Email, Telephone, Adresse, DateCreation)
    VALUES ('Lefevre','Clara','clara.lefevre@example.com','0755123412','21 Boulevard Saint-Michel, Paris', SYSUTCDATETIME());
END

-- Retrieve client IDs
DECLARE @c1 INT = (SELECT TOP 1 Id FROM dbo.Clients WHERE Email='jane.doe@example.com');
DECLARE @c2 INT = (SELECT TOP 1 Id FROM dbo.Clients WHERE Email='john.smith@example.com');
DECLARE @c3 INT = (SELECT TOP 1 Id FROM dbo.Clients WHERE Email='alice.martin@example.com');
DECLARE @c4 INT = (SELECT TOP 1 Id FROM dbo.Clients WHERE Email='bob.durand@example.com');
DECLARE @c5 INT = (SELECT TOP 1 Id FROM dbo.Clients WHERE Email='clara.lefevre@example.com');

-- Commandes (upsert by NumeroCommande)
IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0001')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0001', DATEADD(DAY,-7, SYSUTCDATETIME()), 129.90, 'EnCours', @c1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0002')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0002', DATEADD(DAY,-3, SYSUTCDATETIME()), 89.50, 'Livree', @c1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0003')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0003', DATEADD(DAY,-2, SYSUTCDATETIME()), 59.99, 'EnCours', @c2);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0004')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0004', DATEADD(DAY,-10, SYSUTCDATETIME()), 249.00, 'Annulee', @c3);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0005')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0005', DATEADD(DAY,-1, SYSUTCDATETIME()), 349.00, 'EnCours', @c3);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0006')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0006', DATEADD(DAY,-14, SYSUTCDATETIME()), 19.90, 'Livree', @c4);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0007')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0007', DATEADD(DAY,-5, SYSUTCDATETIME()), 199.99, 'Livree', @c5);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Commandes WHERE NumeroCommande='CMD-2025-0008')
BEGIN
    INSERT INTO dbo.Commandes (NumeroCommande, DateCommande, MontantTotal, Statut, ClientId)
    VALUES ('CMD-2025-0008', DATEADD(HOUR,-6, SYSUTCDATETIME()), 49.00, 'EnCours', @c5);
END