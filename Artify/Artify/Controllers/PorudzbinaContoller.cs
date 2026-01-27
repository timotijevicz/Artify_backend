using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;
using Artify.DTO_klase.PorudzbinaDTO;


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

        /// <summary>
        /// Vraća sve porudžbine.
        /// </summary>
        /// <returns>HTTP 200 sa listom porudžbina.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPorudzbine()
        {
            var porudzbine = await _porudzbinaService.GetAllPorudzbineAsync();
            return Ok(porudzbine);
        }

        /// <summary>
        /// Vraća porudžbinu na osnovu ID-a.
        /// </summary>
        /// <param name="id">ID porudžbine.</param>
        /// <returns>HTTP 200 sa porudžbinom ili HTTP 404 ako nije pronađena.</returns>
        [HttpGet("{PorudzbinaId:int}")]
        public async Task<IActionResult> GetPorudzbinaById(int PorudzbinaId)
        {
            var porudzbina = await _porudzbinaService.GetPorudzbinaByIdAsync(PorudzbinaId);

            if (porudzbina == null)
            {
                return NotFound(new { Poruka = $"Porudžbina sa ID-jem {PorudzbinaId} nije pronađena." });
            }

            return Ok(porudzbina);
        }

        /// <summary>
        /// Kreira novu porudžbinu.
        /// </summary>
        /// <param name="novaPorudzbinaDTO">DTO sa podacima za kreiranje porudžbine.</param>
        /// <returns>HTTP 201 sa kreiranom porudžbinom ili HTTP 400 u slučaju greške.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePorudzbina([FromBody] KreiranjePorudzbineDTO novaPorudzbinaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaPorudzbina = await _porudzbinaService.CreatePorudzbinaAsync(novaPorudzbinaDTO);
            return CreatedAtAction(nameof(GetPorudzbinaById), new { id = novaPorudzbina.PorudzbinaId }, novaPorudzbina);
        }

        /// <summary>
        /// Ažurira postojeću porudžbinu.
        /// </summary>
        /// <param name="id">ID porudžbine.</param>
        /// <param name="izmenaPorudzbineDTO">DTO sa podacima za ažuriranje porudžbine.</param>
        /// <returns>HTTP 204 u slučaju uspešnog ažuriranja, ili HTTP 400/404 u slučaju greške.</returns>
        [HttpPut("{PorudzbinaId:int}")]
        public async Task<IActionResult> UpdatePorudzbina(int PorudzbinaId, [FromBody] AzuriranjePorudzbineDTO izmenaPorudzbineDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            izmenaPorudzbineDTO.PorudzbinaId = PorudzbinaId; // Osiguravamo da se ažurira odgovarajuća porudžbina.
            try
            {
                await _porudzbinaService.UpdatePorudzbinaAsync(izmenaPorudzbineDTO);
            }
            catch (Exception ex)
            {
                return NotFound(new { Poruka = ex.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Briše porudžbinu na osnovu ID-a.
        /// </summary>
        /// <param name="id">ID porudžbine koja se briše.</param>
        /// <returns>HTTP 204 ako je brisanje uspešno, ili HTTP 404 ako porudžbina nije pronađena.</returns>
        [HttpDelete("{PorudzbinaId:int}")]
        public async Task<IActionResult> DeletePorudzbina(int PorudzbinaId)
        {
            var rezultat = await _porudzbinaService.DeletePorudzbinaAsync(PorudzbinaId);

            if (!rezultat)
            {
                return NotFound(new { Poruka = $"Porudžbina sa ID-jem {PorudzbinaId} nije pronađena." });
            }

            return NoContent();
        }
    }

}
