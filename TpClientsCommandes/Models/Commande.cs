using System;

namespace TpClientsCommandes.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public string NumeroCommande { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal MontantTotal { get; set; }
        public string Statut { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}