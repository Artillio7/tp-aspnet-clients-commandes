using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TpClientsCommandes.Data;
using TpClientsCommandes.Models;

namespace TpClientsCommandes.Controllers
{
    [ApiController]
    [Route("api/commandes")]
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
            var cmd = await _db.Commandes.AsNoTracking().Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == id);
            return cmd is null ? NotFound() : Ok(cmd);
        }

        /// <summary>
        /// Crée une commande.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Commande), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Commande>> Create([FromBody] Commande commande)
        {
            var clientExists = await _db.Clients.AnyAsync(c => c.Id == commande.ClientId);
            if (!clientExists)
                return BadRequest(new { message = "ClientId introuvable" });

            _db.Commandes.Add(commande);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = commande.Id }, commande);
        }

        /// <summary>
        /// Met à jour une commande.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] Commande input)
        {
            var cmd = await _db.Commandes.FirstOrDefaultAsync(c => c.Id == id);
            if (cmd is null) return NotFound();

            var clientExists = await _db.Clients.AnyAsync(c => c.Id == input.ClientId);
            if (!clientExists)
                return BadRequest(new { message = "ClientId introuvable" });

            cmd.NumeroCommande = input.NumeroCommande;
            cmd.DateCommande = input.DateCommande;
            cmd.MontantTotal = input.MontantTotal;
            cmd.Statut = input.Statut;
            cmd.ClientId = input.ClientId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Supprime une commande.
        /// </summary>
        [HttpDelete("{id:int}")]
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