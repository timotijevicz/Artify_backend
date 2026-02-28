using Artify.Interfaces;
using Artify.DTO_klase.UmetnickoDeloDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            if (delo == null) return NotFound();

            return Ok(delo);
        }

        [Authorize(Roles = "Umetnik")]
        [HttpGet("MojaDela")]
        public async Task<IActionResult> GetMyArtworks()
        {
            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var dela = await _umetnickoDeloServis.GetArtworksByKorisnikIdAsync(korisnikId);
            return Ok(dela);
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPost("DodajNovoDelo")]
        public async Task<IActionResult> AddArtwork([FromBody] KreirajUmetnickoDeloDTO dto)
        {
            if (dto == null) return BadRequest("DTO je null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            // ✅ pravi umetnikId iz baze (int)
            var umetnikId = await _umetnickoDeloServis.GetUmetnikIdByKorisnikIdAsync(korisnikId);
            if (umetnikId == null) return BadRequest("Umetnik nije pronađen u tabeli Umetnici za ovog korisnika.");

            // ✅ pregazi šta god klijent pošalje
            dto.UmetnikId = umetnikId.Value;

            try
            {
                var kreirano = await _umetnickoDeloServis.AddArtworkAsync(dto);

                return CreatedAtAction(
                    nameof(GetArtworkById),
                    new { id = kreirano.UmetnickoDeloId },
                    kreirano
                );
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPost("DodajNovoDeloZaAukciju")]
        public async Task<IActionResult> AddAuctionArtwork([FromBody] KreirajDeloZaAukcijuDTO dto)
        {
            if (dto == null) return BadRequest("DTO je null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var umetnikId = await _umetnickoDeloServis.GetUmetnikIdByKorisnikIdAsync(korisnikId);
            if (umetnikId == null) return BadRequest("Umetnik nije pronađen.");

            var delo = await _umetnickoDeloServis.AddAuctionArtworkAsync(dto, umetnikId.Value);

            return CreatedAtAction(
                nameof(GetArtworkById),
                new { id = delo.UmetnickoDeloId },
                delo
            );
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPut("AzurirajDelo/{id}")]
        public async Task<IActionResult> UpdateArtwork(int id, [FromBody] AzuriranjeUmetnickogDelaDTO dto)
        {
            if (dto == null) return BadRequest("DTO je null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.UmetnickoDeloId) return BadRequest("ID iz rute se ne poklapa sa DTO ID.");

            var success = await _umetnickoDeloServis.UpdateArtworkAsync(dto);
            if (!success) return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Umetnik, Admin")]
        [HttpDelete("ObrisiDelo/{id}")]
        public async Task<IActionResult> DeleteArtwork(int id)
        {
            var success = await _umetnickoDeloServis.DeleteArtworkAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }


        [Authorize(Roles = "Umetnik")]
        [HttpPut("DeaktivirajDelo/{id}")]
        public async Task<IActionResult> DeaktivirajDelo(int id)
        {
            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var success = await _umetnickoDeloServis.DeactivateArtworkAsync(id, korisnikId);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet("PretragaDela")]
        public async Task<IActionResult> SearchArtworks([FromQuery] string keyword)
        {
            keyword = keyword?.Trim() ?? "";
            if (keyword.Length == 0) return BadRequest("Keyword je prazan.");

            var dela = await _umetnickoDeloServis.SearchArtworksAsync(keyword);
            return Ok(dela);
        }
    }
}
