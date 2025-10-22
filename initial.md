# TP: Gestion des Clients et Commandes avec Validation — ASP.NET Core

Ce document est le briefing de mission initial. Il découpe, étape par étape, tout ce qu’il faut réaliser pour livrer une API REST propre, testée et validée.

---

## PAGE 1 — Briefing général

### Objectifs
- Concevoir une API REST avec ASP.NET Core et Entity Framework Core.
- Manipuler une base de données SQL Server via EF Core.
- Mettre en place une relation One-To-Many entre `Client` et `Commande`.
- Valider les entrées avec DataAnnotations et FluentValidation.

### Contexte
- Un client peut avoir plusieurs commandes; une commande appartient à un seul client.
- L’API doit permettre: ajouter, lister et récupérer un client; ajouter, lister et récupérer une commande; associer une commande à un client; valider les données lors des créations/modifications.

### Livrables attendus
- Projet ASP.NET Core Web API opérationnel.
- Base SQL Server migrée avec les tables `Clients` et `Commandes`.
- Modèles, validations (DataAnnotations + FluentValidation), contrôleurs et endpoints REST.
- Gestion des erreurs cohérente (ProblemDetails / middleware ou filtre).
- Tests unitaires (services) et tests d’intégration (endpoints).

### Pré-requis techniques
- .NET SDK 8 (ou 7), Visual Studio/VS Code.
- SQL Server (LocalDB ou instance) et `dotnet-ef`.
- Packages NuGet: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `FluentValidation`, `FluentValidation.AspNetCore`.

### Étapes à réaliser (vue d’ensemble)
1) Initialiser le projet API et ajouter les dépendances.
2) Modéliser les entités `Client` et `Commande` (One-To-Many).
3) Configurer `DbContext`, la connexion SQL Server et la relation + cascade.
4) Ajouter les validations: DataAnnotations + FluentValidation.
5) Créer les contrôleurs et endpoints REST (GET, POST, PUT, DELETE).
6) Gérer les erreurs de validation et exceptions globales.
7) Écrire les tests unitaires et d’intégration.

---

## 1. Modélisation des entités

Propriétés de navigation nécessaires:
- Dans `Client`: `ICollection<Commande> Commandes`.
- Dans `Commande`: `Client Client` + clé étrangère `ClientId`.

Exemple de modèles (C#):

```csharp
public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Adresse { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}

public class Commande
{
    public int Id { get; set; }
    public string NumeroCommande { get; set; }
    public DateTime DateCommande { get; set; }
    public decimal MontantTotal { get; set; }
    public string Statut { get; set; } // ex: "EnCours", "Payee", "Annulee"

    public int ClientId { get; set; }
    public Client Client { get; set; }
}
```

## 2. Configuration d’Entity Framework Core

- Créer le `DbContext` avec `DbSet<Client>` et `DbSet<Commande>`.
- Configurer la relation et la suppression en cascade.
- Définir la chaîne de connexion dans `appsettings.json` puis créer la migration initiale et mettre à jour la base.

Exemple `DbContext`:

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Commande> Commandes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Commandes)
            .WithOne(cmd => cmd.Client)
            .HasForeignKey(cmd => cmd.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Client>()
            .Property(c => c.Nom).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .Property(c => c.Prenom).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Client>()
            .Property(c => c.Email).IsRequired().HasMaxLength(200);

        modelBuilder.Entity<Commande>()
            .Property(c => c.NumeroCommande).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Commande>()
            .Property(c => c.MontantTotal).HasColumnType("decimal(18,2)");
    }
}
```

`appsettings.json` (exemple):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TpClientsCommandesDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Enregistrement du `DbContext` dans `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

Commandes CLI:

```bash
# Créer le projet API
dotnet new webapi -n TpClientsCommandes
cd TpClientsCommandes

# Ajouter packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore

# Migrations
dotnet tool install -g dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 3. Validation des données

- DataAnnotations (Required, StringLength, EmailAddress, Range, etc.).
- FluentValidation pour règles avancées.

Exemples de DataAnnotations (optionnel si tout est en FluentValidation):

```csharp
public class Client
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Nom { get; set; }
    [Required, StringLength(100)]
    public string Prenom { get; set; }
    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; }
    [StringLength(20)]
    public string Telephone { get; set; }
    [StringLength(300)]
    public string Adresse { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}
```

FluentValidation (exemples):

```csharp
public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prenom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Telephone).MaximumLength(20);
        RuleFor(x => x.Adresse).MaximumLength(300);
    }
}

public class CommandeValidator : AbstractValidator<Commande>
{
    public CommandeValidator()
    {
        RuleFor(x => x.NumeroCommande).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DateCommande).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.MontantTotal).GreaterThan(0);
        RuleFor(x => x.Statut).NotEmpty();
        RuleFor(x => x.ClientId).GreaterThan(0);
    }
}
```

Enregistrement FluentValidation (ex: `Program.cs`):

```csharp
builder.Services.AddControllers().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<ClientValidator>();
});
```

---

## PAGE 2 — Contrôleurs, erreurs, relations, tests

### 4. Contrôleurs et endpoints

- `ClientController`: 
  - GET `/api/clients` (tous)
  - GET `/api/clients/{id}` (un)
  - POST `/api/clients`
  - PUT `/api/clients/{id}`
  - DELETE `/api/clients/{id}`

- `CommandeController`:
  - GET `/api/commandes` (tous)
  - GET `/api/commandes/{id}` (un)
  - POST `/api/commandes`
  - PUT `/api/commandes/{id}`
  - DELETE `/api/commandes/{id}`

- Associer une commande à un client via l’API:
  - Endpoint dédié: POST `/api/clients/{clientId}/commandes` avec le corps de la commande (sans `ClientId` ou en le forçant côté serveur à `clientId`).

### 5. Gestion des erreurs

- Utiliser `[ApiController]` pour la validation automatique du `ModelState` (retours 400).
- Créer un middleware ou filtre d’exception pour unifier les réponses d’erreur (ProblemDetails, logs).
- Renvoyer des codes HTTP appropriés: 400 (validation), 404 (non trouvé), 409 (conflit), 500 (erreur serveur).

### 6. Relations et cascade

- Configurer la cascade: `.OnDelete(DeleteBehavior.Cascade)` pour que supprimer un client supprime ses commandes.
- Tester: créer un client + commandes; supprimer le client; vérifier que les commandes liées disparaissent.

### 7. Tests

- Unitaires (services métier): 
  - Valider création et mise à jour de `Client`/`Commande` avec règles.
  - Utiliser `InMemoryDatabase` ou `Sqlite` en mémoire pour EF.

- Intégration (endpoints API):
  - `WebApplicationFactory<>` pour appeler les endpoints.
  - Vérifier flux: création client → création commande associée → lecture → suppression.

---

## Plan d’exécution détaillé (checklist)

- [ ] Créer projet API: `dotnet new webapi`.
- [ ] Ajouter packages EF + FluentValidation.
- [ ] Créer modèles `Client` et `Commande` + navigation.
- [ ] Créer `AppDbContext` + relation + cascade.
- [ ] Configurer connexion SQL Server (`appsettings.json`).
- [ ] Générer migration et mettre à jour la base.
- [ ] Ajouter DataAnnotations.
- [ ] Ajouter `ClientValidator` et `CommandeValidator`.
- [ ] Enregistrer FluentValidation.
- [ ] Implémenter `ClientController` (GET/POST/PUT/DELETE).
- [ ] Implémenter `CommandeController` (GET/POST/PUT/DELETE).
- [ ] Implémenter POST `/api/clients/{clientId}/commandes`.
- [ ] Ajouter middleware/filtre d’exception.
- [ ] Écrire tests unitaires (services).
- [ ] Écrire tests d’intégration (endpoints).

## Arborescence cible (indicative)

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

## Critères d’acceptation
- Endpoints REST fonctionnels, respectant les codes HTTP.
- Validation côté serveur opérationnelle (DataAnnotations + FluentValidation).
- Suppression en cascade confirmée par test.
- Tests unitaires et d’intégration verts.
- Documentation et structure conformes à ce briefing.