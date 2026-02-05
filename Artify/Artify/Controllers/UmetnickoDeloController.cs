using Artify.Interfaces;
using Artify.DTO_klase.UmetnickoDeloDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Artify.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("SvaDela")]
        public async Task<IActionResult> GetAllArtworks()
        {
            var dela = await _umetnickoDeloServis.GetAllArtworksAsync();
            return Ok(dela);
        }

        [HttpGet("DeloPoID/{id}")]
        public async Task<IActionResult> GetArtworkById(int id)
        {
            var delo = await _umetnickoDeloServis.GetArtworkByIdAsync(id);
            if (delo == null)
                return NotFound();

            return Ok(delo);
        }


        [Authorize(Roles = "Umetnik")]
        [HttpGet("MojaDela")]
        public async Task<IActionResult> GetMyArtworks()
        {
            var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (korisnikId == null) return Unauthorized();

            var dela = await _umetnickoDeloServis
                .GetArtworksByKorisnikIdAsync(korisnikId);

            return Ok(dela);
        }


        [Authorize]
        [HttpPost("DodajNovoDelo")]
        public async Task<IActionResult> AddArtwork(
            [FromBody] KreirajUmetnickoDeloDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var kreirano = await _umetnickoDeloServis.AddArtworkAsync(dto);

            return CreatedAtAction(
                nameof(GetArtworkById),
                new { id = kreirano.UmetnickoDeloId },
                kreirano
            );
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPost("DodajNovoDeloZaAukciju")]
        public async Task<IActionResult> AddAuctionArtwork(
            [FromBody] KreirajDeloZaAukcijuDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var korisnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (korisnikId == null)
                return Unauthorized();

            var dela = await _umetnickoDeloServis
                .GetArtworksByKorisnikIdAsync(korisnikId);

            // izvučemo umetnikId iz prvog dela (ili dodaj posebnu metodu)
            var umetnikId = dela.FirstOrDefault()?.UmetnikId;
            if (umetnikId == null)
                return BadRequest("Umetnik nije pronađen.");

            var delo = await _umetnickoDeloServis
                .AddAuctionArtworkAsync(dto, umetnikId.Value);

            return Ok(delo);
        }


        [Authorize]
        [HttpPut("AzurirajDelo/{id}")]
        public async Task<IActionResult> UpdateArtwork(
            int id,
            [FromBody] AzuriranjeUmetnickogDelaDTO dto)
        {
            if (!ModelState.IsValid || id != dto.UmetnickoDeloId)
                return BadRequest();

            var success = await _umetnickoDeloServis.UpdateArtworkAsync(dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("ObrisiDelo/{id}")]
        public async Task<IActionResult> DeleteArtwork(int id)
        {
            var success = await _umetnickoDeloServis.DeleteArtworkAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

 
        [HttpGet("PretragaDela")]
        public async Task<IActionResult> SearchArtworks([FromQuery] string keyword)
        {
            var dela = await _umetnickoDeloServis.SearchArtworksAsync(keyword);
            return Ok(dela);
        }
    }
}
