using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TpClientsCommandes.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nom { get; set; } = default!;
        [Required, StringLength(100)]
        public string Prenom { get; set; } = default!;
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;
        [StringLength(30)]
        public string Telephone { get; set; } = default!;
        [StringLength(300)]
        public string Adresse { get; set; } = default!;
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}