using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Artify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritiController : ControllerBase
    {
        private readonly IFavoriti _favoritiService;
        private readonly UserManager<Korisnik> _userManager;

        public FavoritiController(IFavoriti favoritiService, UserManager<Korisnik> userManager)
        {
            _favoritiService = favoritiService;
            _userManager = userManager;
        }

        // FavoritiController.cs (izmena samo GetByKupacId)
        [HttpGet("PrikazOmiljenihDelaPoID/{KupacId}")]
        [Authorize(Roles = "Kupac")]
        public async Task<IActionResult> GetByKupacId(string KupacId)
        {
            var tokenKupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(tokenKupacId))
                return Unauthorized(new { message = "Korisnik nije autorizovan." });

            // ✅ ne dozvoli da traži tuđe favorite
            if (KupacId != tokenKupacId)
                return Forbid();

            var korisnik = await _userManager.FindByIdAsync(KupacId);
            if (korisnik == null)
                return NotFound("Korisnik nije pronađen.");

            var favoriti = await _favoritiService.GetAllFavoritesByUserId(KupacId);

            return Ok(favoriti);
        }

        // Dodavanje umetničkog dela u favorite
        [HttpPost("DodajUFavorite/{UmetnickoDeloId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Kupac")]
        public async Task<IActionResult> DodajUFavoriti([FromRoute] int UmetnickoDeloId)
        {
            var KupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(KupacId))
                return Unauthorized(new { message = "Korisnik nije autorizovan." });

            try
            {
                var favorit = await _favoritiService.AddToFavorites(KupacId, UmetnickoDeloId);
                return Ok(favorit);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Uklanjanje umetničkog dela iz favorita
        [HttpDelete("UkloniIzFavorita/{UmetnickoDeloId}")]
        [Authorize(Roles = "Kupac")]
        public async Task<IActionResult> UkloniIzFavorita(int UmetnickoDeloId)
        {
            var KupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(KupacId))
                return Unauthorized(new { message = "Korisnik nije autorizovan." });

            var rezultat = await _favoritiService.RemoveFromFavorites(KupacId, UmetnickoDeloId);
            if (rezultat)
                return NoContent();

            return NotFound("Umetničko delo nije pronađeno u favoritima.");
        }

        // Provera da li je umetničko delo u favoritima korisnika
        [HttpGet("DaLiJeUFavoritima/{UmetnickoDeloId}")]
        [Authorize(Roles = "Kupac")]
        public async Task<IActionResult> DaLiJeUFavoritima(int UmetnickoDeloId)
        {
            var kupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(kupacId))
                return Unauthorized(new { message = "Korisnik nije autorizovan." });

            var postoji = await _favoritiService.Exists(kupacId, UmetnickoDeloId);

            return Ok(postoji); // true / false
        }
    }
}
