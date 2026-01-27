using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;
using Artify.DTO_klase.RecenzijaDTO;


namespace Artify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecenzijaController : ControllerBase
    {
        private readonly IRecenzija _recenzijaService;

        public RecenzijaController(IRecenzija recenzijaService)
        {
            _recenzijaService = recenzijaService;
        }

        // GET: api/Recenzija
        [HttpGet]
        public async Task<IActionResult> GetSveRecenzije()
        {
            var recenzije = await _recenzijaService.GetSveRecenzijeAsync();
            return Ok(recenzije);
        }

        // GET: api/Recenzija/{id}
        [HttpGet("{RecenzijaId}")]
        public async Task<IActionResult> GetRecenzijaById(int RecenzijaId)
        {
            var recenzija = await _recenzijaService.GetRecenzijaByIdAsync(RecenzijaId);
            if (recenzija == null)
            {
                return NotFound($"Recenzija sa ID-om {RecenzijaId} nije pronađena.");
            }
            return Ok(recenzija);
        }

        // GET: api/Recenzija/UmetnickoDelo/{umetnickoDeloId}
        [HttpGet("UmetnickoDelo/{umetnickoDeloId}")]
        public async Task<IActionResult> GetRecenzijeZaUmetnickoDelo(int umetnickoDeloId)
        {
            var recenzije = await _recenzijaService.GetRecenzijeZaUmetnickoDeloAsync(umetnickoDeloId);
            return Ok(recenzije);
        }

        // POST: api/Recenzija
        [HttpPost]
        public async Task<IActionResult> KreirajRecenziju([FromBody] KreirajRecenzijuDTO novaRecenzijaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recenzija = await _recenzijaService.KreirajRecenzijuAsync(novaRecenzijaDTO);
            return CreatedAtAction(nameof(GetRecenzijaById), new { id = recenzija.RecenzijaId }, recenzija);
        }

        // PUT: api/Recenzija/{id}
        [HttpPut("{RecenzijaId}")]
        public async Task<IActionResult> AzurirajRecenziju(int RecenzijaId, [FromBody] AzurirajRecenzijuDTO izmenjenaRecenzijaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (RecenzijaId != izmenjenaRecenzijaDTO.RecenzijaId)
            {
                return BadRequest("ID iz URL-a mora odgovarati ID-u u telu zahteva.");
            }

            try
            {
                var izmenjenaRecenzija = await _recenzijaService.AzurirajRecenzijuAsync(izmenjenaRecenzijaDTO);
                return Ok(izmenjenaRecenzija);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Recenzija/{id}
        [HttpDelete("{RecenzijaId}")]
        public async Task<IActionResult> ObrisiRecenziju(int RecenzijaId)
        {
            var uspesnoObrisana = await _recenzijaService.ObrisiRecenzijuAsync(RecenzijaId);
            if (!uspesnoObrisana)
            {
                return NotFound($"Recenzija sa ID-om {RecenzijaId} nije pronađena.");
            }
            return NoContent();
        }
    }
}
