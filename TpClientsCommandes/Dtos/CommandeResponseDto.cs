using System;

namespace TpClientsCommandes.Dtos
{
    public class CommandeResponseDto
    {
        public int Id { get; set; }
        public required string NumeroCommande { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal MontantTotal { get; set; }
        public required string Statut { get; set; }
        public int ClientId { get; set; }
        public ClientSummaryDto? Client { get; set; }
    }

    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public required string Nom { get; set; }
        public required string Prenom { get; set; }
        public required string Email { get; set; }
        public required string Telephone { get; set; }
        public required string Adresse { get; set; }
        public DateTime DateCreation { get; set; }
    }
}