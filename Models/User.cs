using System.ComponentModel.DataAnnotations;

namespace TpClientsCommandes.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "VIEWER";

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
