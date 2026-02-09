using Artify.Interfaces;
using Microsoft.AspNetCore.Mvc;

using Artify.DTO_klase.PorudzbinaDTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PorudzbinaController : ControllerBase
    {
        private readonly IPorudzbina _porudzbinaService;

        public PorudzbinaController(IPorudzbina porudzbinaService)
        {
            _porudzbinaService = porudzbinaService;
        }

        [HttpGet("VracaSvePorudzbine")]
        public async Task<IActionResult> GetAllPorudzbine()
        {
            var porudzbine = await _porudzbinaService.GetAllPorudzbineAsync();
            return Ok(porudzbine);
        }

        [HttpGet("PorudzbinaPoID/{PorudzbinaId:int}")]
        public async Task<IActionResult> GetPorudzbinaById(int PorudzbinaId)
        {
            var porudzbina = await _porudzbinaService.GetPorudzbinaByIdAsync(PorudzbinaId);

            if (porudzbina == null)
                return NotFound(new { Poruka = $"Porudžbina sa ID-jem {PorudzbinaId} nije pronađena." });

            return Ok(porudzbina);
        }

        [Authorize(Roles = "Kupac")]
        [HttpPost("KreiraNovuPorudzbinu")]
        public async Task<IActionResult> CreatePorudzbina([FromBody] KreiranjePorudzbineDTO novaPorudzbinaDTO)
        {
            // ✅ da ne pada validacija ako neko ostavi non-nullable u DTO
            ModelState.Remove("KorisnikId");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId))
                return Unauthorized();

            novaPorudzbinaDTO.KorisnikId = korisnikId;

            try
            {
                var novaPorudzbina = await _porudzbinaService.CreatePorudzbinaAsync(novaPorudzbinaDTO);

                return CreatedAtAction(
                    nameof(GetPorudzbinaById),
                    new { PorudzbinaId = novaPorudzbina.PorudzbinaId },
                    novaPorudzbina
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { Poruka = ex.Message });
            }
        }

        [HttpPut("AzuriranjePorudzbine/{PorudzbinaId:int}")]
        public async Task<IActionResult> UpdatePorudzbina(
            int PorudzbinaId,
            [FromBody] AzuriranjePorudzbineDTO izmenaPorudzbineDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (PorudzbinaId != izmenaPorudzbineDTO.PorudzbinaId)
                return BadRequest("ID porudžbine ne odgovara DTO.");

            try
            {
                await _porudzbinaService.UpdatePorudzbinaAsync(izmenaPorudzbineDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { Poruka = ex.Message });
            }
        }

        [Authorize(Roles = "Kupac")]
        [HttpGet("MojePorudzbine")]
        public async Task<IActionResult> GetMojePorudzbine()
        {
            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var porudzbine = await _porudzbinaService.GetPorudzbineByKorisnikIdAsync(korisnikId);
            return Ok(porudzbine);
        }

        [Authorize(Roles = "Kupac")]
        [HttpPut("Plati/{porudzbinaId:int}")]
        public async Task<IActionResult> Plati(int porudzbinaId)
        {
            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var ok = await _porudzbinaService.PayAsync(porudzbinaId, korisnikId);
            if (!ok) return NotFound(new { Poruka = "Porudžbina nije pronađena ili nije tvoja." });

            return NoContent();
        }

        [Authorize(Roles = "Kupac")]
        [HttpPut("Arhiviraj/{porudzbinaId:int}")]
        public async Task<IActionResult> Arhiviraj(int porudzbinaId)
        {
            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(korisnikId)) return Unauthorized();

            var ok = await _porudzbinaService.ArchiveAsync(porudzbinaId, korisnikId);
            if (!ok) return BadRequest(new { Poruka = "Ne možeš da arhiviraš ovu porudžbinu." });

            return NoContent();
        }



        [HttpDelete("BrisanjePorudzbine/{PorudzbinaId:int}")]
        public async Task<IActionResult> DeletePorudzbina(int PorudzbinaId)
        {
            var rezultat = await _porudzbinaService.DeletePorudzbinaAsync(PorudzbinaId);

            if (!rezultat)
                return NotFound(new { Poruka = $"Porudžbina sa ID-jem {PorudzbinaId} nije pronađena." });

            return NoContent();
        }
    }
}
