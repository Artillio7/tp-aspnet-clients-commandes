# TP ASP.NET Core — Gestion des Clients et Commandes

Ce dépôt contient une API REST ASP.NET Core pour gérer des clients et des commandes avec SQL Server et Entity Framework Core. Le cadrage détaillé est dans `initial.md`. Cette page résume l’essentiel à retenir et donne les étapes pratiques pour lancer et tester l’API.

## À retenir (essentiel)
- Base SQL Server: instance `\\SQLEXPRESS` locale, base `TpClientsCommandesDb`.
- Chaîne de connexion: `Server=.\\SQLEXPRESS;Database=TpClientsCommandesDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true` (dans `appsettings.json`).
- HTTPS: faire confiance au certificat dev (`dotnet dev-certs https --trust`). Le pop-up Windows “Avertissement de sécurité” est normal en local; cliquez sur `Oui`.
- Documentation: Swagger est accessible sur `https://localhost:7057/swagger` (et `http://localhost:5285/swagger`).
- Validation: DataAnnotations + FluentValidation activées automatiquement (`[ApiController]` + `AddFluentValidationAutoValidation`).
- Endpoints (CRUD):
  - Clients: `GET /api/clients`, `GET /api/clients/{id}`, `POST /api/clients`, `PUT /api/clients/{id}`, `DELETE /api/clients/{id}`.
  - Commandes: `GET /api/commandes?clientId=`, `GET /api/commandes/{id}`, `POST /api/commandes`, `PUT /api/commandes/{id}`, `DELETE /api/commandes/{id}`.

## Prérequis
- .NET SDK 8+ (`dotnet --version` ≥ 8.x). 
- SQL Server Express (service `MSSQL$SQLEXPRESS` en cours d’exécution).
- Outil ligne de commande SQL (`sqlcmd`) pour tests rapides.

## Mise en place de la base
Deux options (si l’outil `dotnet ef` ne fonctionne pas dans votre terminal, utilisez le script):

- Option A (EF Core):
  - `dotnet tool install -g dotnet-ef`
  - `dotnet ef database update`

- Option B (script SQL manuel — déjà validé):
  - `sqlcmd -S .\SQLEXPRESS -i "TpClientsCommandes\Scripts\InitSql.sql"`
  - `sqlcmd -S .\SQLEXPRESS -i "TpClientsCommandes\Scripts\SeedSql.sql"`
  - Crée la base `TpClientsCommandesDb`, les tables `Clients` et `Commandes`, l’index et la FK cascade.

## Lancer l’API
Séquence validée (SDK installé sur `E:\dotnet`) — commandes dans l’ordre:

1) Initialiser et peupler la base SQL (instance locale `\\SQLEXPRESS`):
   - `sqlcmd -S .\SQLEXPRESS -i "TpClientsCommandes\Scripts\InitSql.sql"`
   - `sqlcmd -S .\SQLEXPRESS -i "TpClientsCommandes\Scripts\SeedSql.sql"`

2) Préparer l’environnement .NET (session PowerShell):
   - `$env:DOTNET_ROOT='E:\dotnet'`
   - `$env:PATH='E:\dotnet;' + $env:PATH`
   - (option de vérification) `E:\dotnet\dotnet.exe --list-sdks`

3) Faire confiance au certificat HTTPS de dev:
   - `E:\dotnet\dotnet.exe dev-certs https --trust`

4) Restaurer et démarrer l’API (répertoire `TpClientsCommandes`):
   - `E:\dotnet\dotnet.exe restore`
   - `E:\dotnet\dotnet.exe run --launch-profile https`

5) Ouvrir la documentation Swagger:
   - `https://localhost:7057/swagger` (HTTPS) — alternatif HTTP: `http://localhost:5285/swagger`

6) Vérifications rapides (terminal):
   - `curl.exe -s -k https://localhost:7057/api/clients`
   - `curl.exe -s -k "https://localhost:7057/api/commandes?clientId=1"`

## Exemples de payloads Swagger
- POST `/api/clients`
```
{
  "Nom": "Doe",
  "Prenom": "John",
  "Email": "john@example.com",
  "Telephone": "0123456789",
  "Adresse": "123 Rue Ex"
}
```

- POST `/api/commandes` (après création de client, ex: `ClientId = 1`)
```
{
  "NumeroCommande": "CMD-2025-0001",
  "DateCommande": "2025-10-23T10:00:00Z",
  "MontantTotal": 99.90,
  "Statut": "EnCours",
  "ClientId": 1
}
```

## Structure du projet
```
TpClientsCommandes/
  Controllers/
    ClientsController.cs
    CommandesController.cs
  Models/
    Client.cs
    Commande.cs
  Data/
    AppDbContext.cs
  Validators/
    ClientValidator.cs
    CommandeValidator.cs
  Scripts/
    InitSql.sql
  Program.cs
  appsettings.json
```

## Validation et erreurs
- DataAnnotations: `Required`, `StringLength`, `EmailAddress`, `Range`.
- FluentValidation: règles avancées sur `Client` et `Commande`.
- `[ApiController]` + auto-validation: renvoie `400 Bad Request` avec détails en cas de payload invalide.

## Dépannage rapide
- Vérifier SQL Server:
  - `sqlcmd -S .\SQLEXPRESS -Q "SELECT @@VERSION"`
  - Service: `Get-Service -Name 'MSSQL$SQLEXPRESS'`
- Instance nommée: si `SQLBrowser` est arrêté/désactivé, l’accès par nom peut échouer. 
  - Solution préférée: démarrer `SQLBrowser` (admin requis).
  - Alternative: fixer un port statique et utiliser `Server=localhost,1433;...`.
- Certificat HTTPS non approuvé:
  - `dotnet dev-certs https --trust` puis accepter le pop-up Windows (cliquer `Oui`).
- `dotnet ef` introuvable: installer l’outil en global, ou utiliser le script SQL.

## Commandes utiles
- Build: `dotnet build`
- Run: `dotnet run --launch-profile https`
- Tool EF: `dotnet tool install -g dotnet-ef`
- Migration: `dotnet ef migrations add InitialCreate`
- Update DB: `dotnet ef database update`
- Script manuel DB: `sqlcmd -S .\SQLEXPRESS -i "TpClientsCommandes\Scripts\InitSql.sql"`

## Licence
Projet académique. Toute réutilisation doit respecter le cadre pédagogique défini.