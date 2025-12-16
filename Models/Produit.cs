using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TpClientsCommandes.Models
{
    public class Produit
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Libelle { get; set; } = default!;

        [Range(0.01, 1000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrixUnitaire { get; set; }

        public int Stock { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
