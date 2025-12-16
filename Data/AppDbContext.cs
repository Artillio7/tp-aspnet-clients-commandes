using Microsoft.EntityFrameworkCore;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Commande> Commandes => Set<Commande>();
        public DbSet<Produit> Produits => Set<Produit>();
        public DbSet<LigneCommande> LignesCommande => Set<LigneCommande>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Nom).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Prenom).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Telephone).HasMaxLength(30);
                entity.Property(e => e.Adresse).HasMaxLength(300);

                entity.HasMany(e => e.Commandes)
                      .WithOne(c => c.Client)
                      .HasForeignKey(c => c.ClientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Commande>(entity =>
            {
                entity.Property(e => e.NumeroCommande).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Statut).HasMaxLength(50).IsRequired();
                entity.Property(e => e.MontantTotal).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.LignesCommande)
                      .WithOne(l => l.Commande)
                      .HasForeignKey(l => l.CommandeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Produit>(entity =>
            {
                entity.Property(e => e.Libelle).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PrixUnitaire).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Nom).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Prenom).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}