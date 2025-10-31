
























































































































































































































































































































































































using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TpClientsCommandes.Data;
using TpClientsCommandes.Models;
using TpClientsCommandes.Dtos;

namespace TpClientsCommandes.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ClientsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Liste des clients.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetAll()
        {
            var clients = await _db.Clients
                .AsNoTracking()
                .Include(c => c.Commandes)
                .OrderBy(c => c.Nom)
                .ToListAsync();

            var dtos = clients.Select(MapClientToResponseDto).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Détail d'un client par id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientResponseDto>> GetById(int id)
        {
            var client = await _db.Clients.AsNoTracking()
                .Include(c => c.Commandes)
                .FirstOrDefaultAsync(c => c.Id == id);
            return client is null ? NotFound() : Ok(MapClientToResponseDto(client));
        }

        /// <summary>
        /// Crée un client.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClientResponseDto>> Create([FromBody] Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            // Chargement éventuel des commandes (vide à la création) non nécessaire
            var dto = MapClientToResponseDto(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, dto);
        }

        /// <summary>
        /// Met à jour un client existant.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] Client input)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return NotFound();

            client.Nom = input.Nom;
            client.Prenom = input.Prenom;
            client.Email = input.Email;
            client.Telephone = input.Telephone;
            client.Adresse = input.Adresse;
            // DateCreation conservée

            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Supprime un client.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return NotFound();

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static ClientResponseDto MapClientToResponseDto(Client c)
        {
            return new ClientResponseDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Prenom = c.Prenom,
                Email = c.Email,
                Telephone = c.Telephone,
                Adresse = c.Adresse,
                DateCreation = c.DateCreation,
                Commandes = (c.Commandes ?? new List<Models.Commande>())
                    .Where(co => co is not null)
                    .Select(co => new CommandeSummaryDto
                    {
                        Id = co.Id,
                        NumeroCommande = co.NumeroCommande,
                        DateCommande = co.DateCommande,
                        MontantTotal = co.MontantTotal,
                        Statut = co.Statut,
                        ClientId = co.ClientId
                    })
                    .ToList()
            };
        }
    }
}