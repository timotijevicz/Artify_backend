using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;
using Artify.DTO_klase.UmetnickoDeloDTO;


namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UmetnickoDeloController : ControllerBase
    {
        private readonly IUmetnickoDelo _umetnickoDeloServis;

        public UmetnickoDeloController(IUmetnickoDelo umetnickoDeloServis)
        {
            _umetnickoDeloServis = umetnickoDeloServis;
        }

        // Vraća sva umetnička dela
        [HttpGet]
        public async Task<IActionResult> GetAllArtworks()
        {
            var dela = await _umetnickoDeloServis.GetAllArtworksAsync();
            return Ok(dela);
        }

        // Vraća umetničko delo po ID-u
        [HttpGet("{UmetnickoDeloId}")]
        public async Task<IActionResult> GetArtworkById(int UmetnickoDeloId)
        {
            var delo = await _umetnickoDeloServis.GetArtworkByIdAsync(UmetnickoDeloId);
            if (delo == null)
            {
                return NotFound("Umetničko delo sa datim ID-om nije pronađeno.");
            }
            return Ok(delo);
        }

        // Dodaje novo umetničko delo
        [HttpPost]
        public async Task<IActionResult> AddArtwork([FromBody] KreirajUmetnickoDeloDTO novoDelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kreiranoDelo = await _umetnickoDeloServis.AddArtworkAsync(novoDelo);
            return CreatedAtAction(nameof(GetArtworkById), new { id = kreiranoDelo.UmetnickoDeloId }, kreiranoDelo);
        }

        // Ažurira postojeće umetničko delo
        [HttpPut("{UmetnickoDeloId}")]
        public async Task<IActionResult> UpdateArtwork(int UmetnickoDeloId, [FromBody] AzuriranjeUmetnickogDelaDTO izmenjenoDelo)
        {
            if (!ModelState.IsValid || UmetnickoDeloId != izmenjenoDelo.UmetnickoDeloId)
            {
                return BadRequest("Podaci nisu ispravni.");
            }

            var uspeh = await _umetnickoDeloServis.UpdateArtworkAsync(izmenjenoDelo);
            if (!uspeh)
            {
                return NotFound("Umetničko delo sa datim ID-om nije pronađeno.");
            }

            return NoContent();
        }

        // Briše umetničko delo po ID-u
        [HttpDelete("{UmetnickoDeloId}")]
        public async Task<IActionResult> DeleteArtwork(int UmetnickoDeloId)
        {
            var uspeh = await _umetnickoDeloServis.DeleteArtworkAsync(UmetnickoDeloId);
            if (!uspeh)
            {
                return NotFound("Umetničko delo sa datim ID-om nije pronađeno.");
            }

            return NoContent();
        }

        // Vraća umetnička dela prema kategoriji
        [HttpGet("by-category/{ategorijaId}")]
        public async Task<IActionResult> GetArtworksByCategory(int KategorijaId)
        {
            var dela = await _umetnickoDeloServis.GetArtworksByCategoryAsync(KategorijaId);
            return Ok(dela);
        }

        // Vraća umetnička dela prema umetniku
        [HttpGet("by-artist/{UmetnikId}")]
        public async Task<IActionResult> GetArtworksByArtist(string UmetnikId)
        {
            var dela = await _umetnickoDeloServis.GetArtworksByArtistAsync(UmetnikId);
            return Ok(dela);
        }

        // Pretražuje umetnička dela prema ključnoj reči
        [HttpGet("search")]
        public async Task<IActionResult> SearchArtworks([FromQuery] string keyword)
        {
            var dela = await _umetnickoDeloServis.SearchArtworksAsync(keyword);
            return Ok(dela);
        }
    }
}
