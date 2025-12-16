using System;
using System.ComponentModel.DataAnnotations;

namespace TpClientsCommandes.Models
{
    public class Commande
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string NumeroCommande { get; set; } = default!;
        public DateTime DateCommande { get; set; }
        [Range(0, double.MaxValue)]
        public decimal MontantTotal { get; set; }
        [Required, StringLength(50)]
        public string Statut { get; set; } = default!;

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
    }
}