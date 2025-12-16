using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpClientsCommandes.Models
{
    public class LigneCommande
    {
        public int Id { get; set; }

        public int CommandeId { get; set; }
        public Commande? Commande { get; set; }

        public int ProduitId { get; set; }
        public Produit? Produit { get; set; }

        [Range(1, 1000)]
        public int Quantite { get; set; }
    }
}
