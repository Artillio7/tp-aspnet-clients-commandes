# TP ASP.NET Core — Gestion des Clients et Commandes

Ce dépôt documente et prépare la réalisation d’une API REST permettant de gérer des clients et leurs commandes avec ASP.NET Core et Entity Framework Core. Le briefing détaillé est disponible dans `initial.md`.

## Aperçu
- API REST pour créer, consulter, mettre à jour et supprimer des clients et des commandes.
- Relation One‑To‑Many: un client possède plusieurs commandes; une commande appartient à un seul client.
- Validation des entrées: Data Annotations et FluentValidation.
- Base de données SQL Server gérée via EF Core (migrations).

## Objectifs pédagogiques
- Concevoir une API REST maintenable, testée et documentée.
- Modéliser correctement les entités et leurs relations.
- Mettre en place des validations et une gestion d’erreurs cohérente.
- Écrire des tests unitaires et d’intégration.

## Périmètre fonctionnel
- Clients: `GET /api/clients`, `GET /api/clients/{id}`, `POST /api/clients`, `PUT /api/clients/{id}`, `DELETE /api/clients/{id}`.
- Commandes: `GET /api/commandes`, `GET /api/commandes/{id}`, `POST /api/commandes`, `PUT /api/commandes/{id}`, `DELETE /api/commandes/{id}`.
- Association d’une commande à un client: `POST /api/clients/{clientId}/commandes`.

## Pile technique
- ASP.NET Core Web API (C# / .NET 7 ou 8)
- Entity Framework Core + SQL Server
- FluentValidation
- Tests: xUnit / MSTest + `WebApplicationFactory` pour intégration

## Mise en route (préparation du projet)
1) Créer le projet API:
   - `dotnet new webapi -n TpClientsCommandes`
2) Ajouter les packages NuGet:
   - `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
   - `dotnet add package Microsoft.EntityFrameworkCore.Tools`
   - `dotnet add package FluentValidation`
   - `dotnet add package FluentValidation.AspNetCore`
3) Configurer `AppDbContext` et la chaîne de connexion dans `appsettings.json`.
4) Générer la migration initiale et mettre à jour la base:
   - `dotnet tool install -g dotnet-ef`
   - `dotnet ef migrations add InitialCreate`
   - `dotnet ef database update`

Les étapes et exemples de code sont détaillés dans `initial.md`.

## Structure cible
```
TpClientsCommandes/
  Controllers/
    ClientController.cs
    CommandeController.cs
  Models/
    Client.cs
    Commande.cs
  Data/
    AppDbContext.cs
  Validators/
    ClientValidator.cs
    CommandeValidator.cs
  appsettings.json
  Program.cs
```

## Validation et erreurs
- Data Annotations pour les contraintes simples (Required, StringLength, EmailAddress, Range).
- FluentValidation pour les règles avancées.
- Gestion unifiée des erreurs via middleware ou filtre d’exception + `ProblemDetails`.
- Codes HTTP cohérents: 400 (validation), 404 (non trouvé), 409 (conflit), 500 (erreur serveur).

## Suppression en cascade
- Configuration EF: `OnDelete(DeleteBehavior.Cascade)` entre `Client` et `Commande`.
- Tests: suppression d’un client entraîne la suppression de ses commandes.

## Tests
- Unitaires: règles métier et validations.
- Intégration: endpoints API avec `WebApplicationFactory`.

## Mise en ligne GitHub
- Propriétaire: `Artillio7`
- Collaborateur enseignant: `nt37`
- Recommandation de nom de dépôt: `tp-aspnet-clients-commandes`

### Création du dépôt et push
1) Créer le dépôt sur GitHub (`https://github.com/new`) avec le nom `tp-aspnet-clients-commandes` (visibilité: `private` conseillée).
2) Ajouter le remote et pousser:
   - `git remote add origin https://github.com/Artillio7/tp-aspnet-clients-commandes.git`
   - `git push -u origin main`
3) Ajouter le collaborateur (interface GitHub):
   - Dépôt → `Settings` → `Collaborators` → `Add people` → rechercher `nt37` → envoyer l’invitation.

## Assistance IA
Ce projet bénéficie d’un appui d’assistance IA pour la rédaction et le cadrage technique (pair programming). Les propositions générées sont systématiquement revues, adaptées au contexte et validées avant intégration. L’IA ne remplace pas les bonnes pratiques de conception, de revue et de test.

## Licence
Projet académique. Toute réutilisation doit respecter le cadre pédagogique défini par l’enseignant.