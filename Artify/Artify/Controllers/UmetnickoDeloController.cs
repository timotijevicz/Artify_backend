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
        [HttpGet("SvaDela")]
        public async Task<IActionResult> GetAllArtworks()
        {
            var dela = await _umetnickoDeloServis.GetAllArtworksAsync();
            return Ok(dela);
        }

        // Vraća umetničko delo po ID-u
        [HttpGet("DeloPoID/{UmetnickoDeloId}")]
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
        [HttpPost("DodajNovoDelo")]
        public async Task<IActionResult> AddArtwork([FromBody] KreirajUmetnickoDeloDTO novoDelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kreiranoDelo = await _umetnickoDeloServis.AddArtworkAsync(novoDelo);
            return CreatedAtAction(
                nameof(GetArtworkById),
                new { UmetnickoDeloId = kreiranoDelo.UmetnickoDeloId },
                kreiranoDelo
            );

        }

        // Ažurira postojeće umetničko delo
        [HttpPut("AzurirajDelo/{UmetnickoDeloId}")]
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
        [HttpDelete("ObrisiDelo/{UmetnickoDeloId}")]
        public async Task<IActionResult> DeleteArtwork(int UmetnickoDeloId)
        {
            var uspeh = await _umetnickoDeloServis.DeleteArtworkAsync(UmetnickoDeloId);
            if (!uspeh)
            {
                return NotFound("Umetničko delo sa datim ID-om nije pronađeno.");
            }

            return NoContent();
        }


        // Vraća umetnička dela prema umetniku
        [HttpGet("DelaPoIDUmetnika/{UmetnikId}")]
        public async Task<IActionResult> GetArtworksByArtist(int UmetnikId)
        {
            var dela = await _umetnickoDeloServis.GetArtworksByArtistAsync(UmetnikId);
            return Ok(dela);
        }

        // Pretražuje umetnička dela prema ključnoj reči
        [HttpGet("PretragaDela")]
        public async Task<IActionResult> SearchArtworks([FromQuery] string keyword)
        {
            var dela = await _umetnickoDeloServis.SearchArtworksAsync(keyword);
            return Ok(dela);
        }
    }
}
