using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;
using Artify.DTO_klase.KategorijeDTO;

namespace Artify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategorijaController : ControllerBase
    {
        private readonly IKategorija _kategorijaService;

        public KategorijaController(IKategorija kategorijaService)
        {
            _kategorijaService = kategorijaService;
        }

        // Pribavljanje svih kategorija
        [HttpGet]
        public async Task<IActionResult> GetAllKategorije()
        {
            var kategorije = await _kategorijaService.GetAllKategorijeAsync();
            return Ok(kategorije);
        }

        // Pribavljanje kategorije po ID-u
        [HttpGet("KategorijaDohvati/{KategorijaId}")]
        public async Task<IActionResult> GetKategorijaById(int KategorijaId)
        {
            var kategorija = await _kategorijaService.GetKategorijaByIdAsync(KategorijaId);
            if (kategorija == null)
            {
                return NotFound("Kategorija nije pronađena.");
            }
            return Ok(kategorija);
        }

        // Kreiranje nove kategorije
        [HttpPost("KreiranjeKategorije")]
        public async Task<IActionResult> CreateKategorija([FromBody] KreiranjeKategorijeDTO novaKategorijaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaKategorija = await _kategorijaService.CreateKategorijaAsync(novaKategorijaDTO);
            return CreatedAtAction(nameof(GetKategorijaById), new { id = novaKategorija.KategorijaId }, novaKategorija);
        }

        // Ažuriranje postojeće kategorije
        [HttpPut]
        public async Task<IActionResult> UpdateKategorija([FromBody] AzuriranjeKategorijeDTO izmenjenaKategorijaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var kategorija = await _kategorijaService.UpdateKategorijaAsync(izmenjenaKategorijaDTO);

            if (kategorija == null)
            {
                return NotFound("Kategorija nije pronađena.");
            }

            return Ok(kategorija);
        }

        // Brisanje kategorije po ID-u
        [HttpDelete("{KategorijaId}")]
        public async Task<IActionResult> DeleteKategorija(int KategorijaId)
        {
            var obrisano = await _kategorijaService.DeleteKategorijaAsync(KategorijaId);

            if (!obrisano)
            {
                return NotFound("Kategorija nije pronađena.");
            }

            return NoContent();
        }

    }
}
