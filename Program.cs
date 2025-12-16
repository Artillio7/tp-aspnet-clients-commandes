using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TpClientsCommandes.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using TpClientsCommandes.Models;
using TpClientsCommandes.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ArtillioBoutiqueSecretKey2024VeryLongKeyForSecurityPurposes";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ArtillioBoutique",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ArtillioBoutiqueApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers & FluentValidation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TpClientsCommandes.Validators.ClientValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        // Create Users table if it doesn't exist
        db.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Nom NVARCHAR(100) NOT NULL,
                    Prenom NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(200) NOT NULL,
                    PasswordHash NVARCHAR(MAX) NOT NULL,
                    Role NVARCHAR(50) NOT NULL,
                    DateCreation DATETIME2 NOT NULL DEFAULT GETDATE(),
                    IsActive BIT NOT NULL DEFAULT 1
                );
                CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
            END
        ");

        if (!db.Clients.Any())
        {
            var clients = new[]
            {
                new Client { Nom = "Artillio", Prenom = "Boutique", Email = "contact@artillio.com", Telephone = "0101010101", Adresse = "1 Rue du Code, Paris" },
                new Client { Nom = "Doe", Prenom = "Jane", Email = "jane.doe@example.com", Telephone = "0202020202", Adresse = "2 Avenue Dev, Lyon" }
            };
            db.Clients.AddRange(clients);
            db.SaveChanges();
        }

        if (!db.Produits.Any())
        {
            var produits = new[]
            {
                new Produit { Libelle = "T-shirt logo", PrixUnitaire = 19.99m, Stock = 100, Description = "T-shirt Artillio", ImageUrl = "/images/tshirt.jpg" },
                new Produit { Libelle = "Mug", PrixUnitaire = 9.99m, Stock = 200, Description = "Mug personnalisé", ImageUrl = "/images/mug.jpg" },
                new Produit { Libelle = "Sticker pack", PrixUnitaire = 4.99m, Stock = 500, Description = "Stickers Artillio", ImageUrl = "/images/stickers.jpg" }
            };
            db.Produits.AddRange(produits);
            db.SaveChanges();
        }

        if (!db.Commandes.Any())
        {
            var client = db.Clients.First();
            var p1 = db.Produits.First();
            var p2 = db.Produits.Skip(1).First();

            var commande = new Commande
            {
                NumeroCommande = "CMD-0001",
                DateCommande = DateTime.UtcNow,
                Statut = "Créée",
                ClientId = client.Id,
                MontantTotal = p1.PrixUnitaire * 2 + p2.PrixUnitaire * 1,
                LignesCommande = new List<LigneCommande>
                {
                    new LigneCommande { ProduitId = p1.Id, Quantite = 2 },
                    new LigneCommande { ProduitId = p2.Id, Quantite = 1 }
                }
            };

            db.Commandes.Add(commande);
            db.SaveChanges();
        }

        // Seed Users
        if (!db.Users.Any())
        {
            var users = new[]
            {
                new User
                {
                    Nom = "Junior",
                    Prenom = "Artillio",
                    Email = "artilliojunior@gmail.com",
                    PasswordHash = AuthController.HashPassword("Artillio2001."),
                    Role = "ADMIN"
                },
                new User
                {
                    Nom = "Kadir",
                    Prenom = "Gestionnaire",
                    Email = "kadir@gmail.com",
                    PasswordHash = AuthController.HashPassword("Artillio2001."),
                    Role = "GESTIONNAIRE_COMMANDES"
                },
                new User
                {
                    Nom = "NT37",
                    Prenom = "Gestionnaire",
                    Email = "nt37@gmail.com",
                    PasswordHash = AuthController.HashPassword("Artillio2001."),
                    Role = "GESTIONNAIRE_CLIENTS"
                },
                new User
                {
                    Nom = "Visiteur",
                    Prenom = "User",
                    Email = "visiteur@gmail.com",
                    PasswordHash = AuthController.HashPassword("Artillio2001."),
                    Role = "VIEWER"
                }
            };
            db.Users.AddRange(users);
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred during database seeding: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
