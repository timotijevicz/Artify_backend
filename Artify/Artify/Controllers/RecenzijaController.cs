using Artify.Interfaces;
using Artify.DTO_klase.RecenzijaDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecenzijaController : ControllerBase
    {
        private readonly IRecenzija _recenzijaRepo;

        public RecenzijaController(IRecenzija recenzijaRepo)
        {
            _recenzijaRepo = recenzijaRepo;
        }

        [HttpGet("VracaSveRecenzije")]
        public async Task<IActionResult> GetSve()
        {
            var list = await _recenzijaRepo.GetSveRecenzijeAsync();
            return Ok(list);
        }

        [HttpGet("RecenzijaPoID/{RecenzijaId}")]
        public async Task<IActionResult> GetById(int RecenzijaId)
        {
            var rec = await _recenzijaRepo.GetRecenzijaByIdAsync(RecenzijaId);
            if (rec == null) return NotFound();
            return Ok(rec);
        }

        [HttpGet("RecenzijaZaUmetnickoDelo/{umetnickoDeloId}")]
        public async Task<IActionResult> GetZaDelo(int umetnickoDeloId)
        {
            var list = await _recenzijaRepo.GetRecenzijeZaUmetnickoDeloAsync(umetnickoDeloId);
            return Ok(list);
        }

        [Authorize(Roles = "Kupac")]
        [HttpPost("KreiranjeRecenzije")]
        public async Task<IActionResult> Kreiraj([FromBody] KreirajRecenzijuDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (korisnikId == null)
                return Unauthorized();

            dto.KorisnikId = korisnikId;

            var rec = await _recenzijaRepo.KreirajRecenzijuAsync(dto);
            return Ok(rec);
        }

        [Authorize(Roles = "Kupac")]
        [HttpPut("AzuriranjeRecenzijePoID/{RecenzijaId}")]
        public async Task<IActionResult> Azuriraj(
            int RecenzijaId,
            [FromBody] AzurirajRecenzijuDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (RecenzijaId != dto.RecenzijaId)
                return BadRequest("ID u ruti i telu se ne poklapaju.");

            var rec = await _recenzijaRepo.AzurirajRecenzijuAsync(dto);
            return Ok(rec);
        }

        [Authorize(Roles = "Kupac,Admin")]
        [HttpDelete("BrisanjeRecenzije/{RecenzijaId}")]
        public async Task<IActionResult> Obrisi(int RecenzijaId)
        {
            var ok = await _recenzijaRepo.ObrisiRecenzijuAsync(RecenzijaId);
            return ok ? NoContent() : NotFound();
        }
    }
}
