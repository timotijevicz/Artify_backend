using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.DTO_klase.UmetnikDTO;
using Artify.Repositories;
using System.Security.Claims;

namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UmetnikController : ControllerBase
    {
        private readonly IUmetnik _umetnikService;

        public UmetnikController(IUmetnik umetnikService)
        {
            _umetnikService = umetnikService;
        }

        /// <summary>
        /// Vraća sve umetnike sa njihovim podacima i delima.
        /// </summary>
        [HttpGet("SviUmetnici")]
        public async Task<IActionResult> GetAllArtists()
        {
            var umetnici = await _umetnikService.GetAllArtistsAsync();
            return Ok(umetnici);
        }

        /// <summary>
        /// Vraća jednog umetnika po ID-u.
        /// </summary>
        [Authorize]
        [HttpGet("VracaUmetnikaPoID/{UmetnikId:int}")]
        public async Task<IActionResult> GetArtistById(int UmetnikId)
        {
            try
            {
                var umetnik = await _umetnikService.GetArtistByIdAsync(UmetnikId);
                if (umetnik == null)
                    return NotFound(new { Poruka = "Umetnik nije pronađen." });

                return Ok(umetnik);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message, ex.StackTrace });
            }
        }

        /// <summary>
        /// Kreira novog umetnika (registracija).
        /// </summary>
        [HttpPost("KreirajUmetnika")]
        public async Task<IActionResult> CreateArtist([FromBody] KreirajUmetnikaDTO noviUmetnik)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var umetnik = await _umetnikService.CreateArtistAsync(noviUmetnik);
            return CreatedAtAction(nameof(GetArtistById), new { UmetnikId = umetnik.UmetnikId }, umetnik);
        }

        /// <summary>
        /// Ažurira podatke o umetniku.
        /// </summary>
        [HttpPut("AzurirajUmetnika")]
        public async Task<IActionResult> UpdateArtist([FromBody] AzurirajUmetnikaDTO izmenaUmetnika)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uspeh = await _umetnikService.UpdateArtistAsync(izmenaUmetnika);
            if (!uspeh)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            return NoContent();
        }

        /// <summary>
        /// Brisanje umetnika po ID-u.
        /// </summary>
        [HttpDelete("ObrisiUmetnika/{UmetnikId}")]
        public async Task<IActionResult> DeleteArtist(int UmetnikId)
        {
            var uspeh = await _umetnikService.DeleteArtistAsync(UmetnikId);
            if (!uspeh)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            return NoContent();
        }

        /// <summary>
        /// Odobri umetnika (admin funkcija).
        /// </summary>
        [HttpPost("OdobriUmetnika/{UmetnikId}")]
        public async Task<IActionResult> ApproveArtist(int UmetnikId)
        {
            var uspeh = await _umetnikService.ApproveArtistAsync(UmetnikId);
            if (!uspeh)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            return Ok(new { Poruka = "Umetnik odobren i dostupan za prikaz." });
        }
    }
}
