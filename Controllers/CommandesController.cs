using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TpClientsCommandes.Data;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Controllers
{
    [ApiController]
    [Route("api/commandes")]
    [Authorize]
    public class CommandesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CommandesController(AppDbContext db) => _db = db;

        /// <summary>
        /// Liste des commandes (filtrable par clientId).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Commande>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Commande>>> GetAll([FromQuery] int? clientId)
        {
            var query = _db.Commandes.AsNoTracking().Include(c => c.Client).AsQueryable();
            if (clientId.HasValue)
                query = query.Where(c => c.ClientId == clientId.Value);
            var list = await query.OrderByDescending(c => c.DateCommande).ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Détail d'une commande.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Commande), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Commande>> GetById(int id)
        {
            var cmd = await _db.Commandes.AsNoTracking()
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Produit)
                .FirstOrDefaultAsync(c => c.Id == id);
            return cmd is null ? NotFound() : Ok(cmd);
        }

        /// <summary>
        /// Crée une commande.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,GESTIONNAIRE_COMMANDES")]
        [ProducesResponseType(typeof(Commande), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Commande>> Create([FromBody] Commande commande)
        {
            var clientExists = await _db.Clients.AnyAsync(c => c.Id == commande.ClientId);
            if (!clientExists)
                return BadRequest(new { message = "ClientId introuvable" });

            if (commande.LignesCommande != null && commande.LignesCommande.Any())
            {
                decimal total = 0m;
                foreach (var ligne in commande.LignesCommande)
                {
                    var produit = await _db.Produits.FindAsync(ligne.ProduitId);
                    if (produit == null)
                        return BadRequest(new { message = $"Produit {ligne.ProduitId} introuvable" });
                    total += produit.PrixUnitaire * ligne.Quantite;
                }
                commande.MontantTotal = total;
            }
            else
            {
                commande.MontantTotal = 0m;
            }

            if (commande.DateCommande == default)
                commande.DateCommande = DateTime.UtcNow;

            _db.Commandes.Add(commande);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = commande.Id }, commande);
        }

        /// <summary>
        /// Met à jour une commande.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN,GESTIONNAIRE_COMMANDES")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] Commande input)
        {
            var cmd = await _db.Commandes.Include(c => c.LignesCommande).FirstOrDefaultAsync(c => c.Id == id);
            if (cmd is null) return NotFound();

            var clientExists = await _db.Clients.AnyAsync(c => c.Id == input.ClientId);
            if (!clientExists)
                return BadRequest(new { message = "ClientId introuvable" });

            cmd.NumeroCommande = input.NumeroCommande;
            cmd.DateCommande = input.DateCommande;
            cmd.Statut = input.Statut;
            cmd.ClientId = input.ClientId;

            var existingByProduit = cmd.LignesCommande.ToDictionary(l => l.ProduitId);
            var inputLines = input.LignesCommande ?? new List<LigneCommande>();

            foreach (var l in inputLines)
            {
                var produit = await _db.Produits.FindAsync(l.ProduitId);
                if (produit == null)
                    return BadRequest(new { message = $"Produit {l.ProduitId} introuvable" });

                if (existingByProduit.TryGetValue(l.ProduitId, out var existing))
                {
                    existing.Quantite = l.Quantite;
                }
                else
                {
                    cmd.LignesCommande.Add(new LigneCommande { ProduitId = l.ProduitId, Quantite = l.Quantite });
                }
            }

            var inputProduitIds = inputLines.Select(x => x.ProduitId).ToHashSet();
            var toRemove = cmd.LignesCommande.Where(l => !inputProduitIds.Contains(l.ProduitId)).ToList();
            foreach (var r in toRemove)
                _db.LignesCommande.Remove(r);

            decimal total = 0m;
            foreach (var l in cmd.LignesCommande)
            {
                var produit = await _db.Produits.FindAsync(l.ProduitId);
                if (produit != null)
                    total += produit.PrixUnitaire * l.Quantite;
            }
            cmd.MontantTotal = total;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Supprime une commande.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN,GESTIONNAIRE_COMMANDES")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var cmd = await _db.Commandes.FirstOrDefaultAsync(c => c.Id == id);
            if (cmd is null) return NotFound();

            _db.Commandes.Remove(cmd);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
