//using Artify.Interfaces;
//using Artify.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Artify.Repositories;
//using System.Security.Claims;

//namespace Artify.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class FavoritiController : ControllerBase
//    {
//        private readonly IFavoriti _favoritiService;
//        private readonly UserManager<Korisnik> _userManager;

//        public FavoritiController(IFavoriti favoritiService, UserManager<Korisnik> userManager)
//        {
//            _favoritiService = favoritiService;
//            _userManager = userManager;
//        }

//        // Prikaz svih omiljenih umetničkih dela po ID korisnika
//        [HttpGet("PrikazOmiljenihDelaPoID/{KupacId}")]
//        [Authorize(Roles = "Kupac")]
//        public async Task<IActionResult> GetByKupacId(string KupacId)
//        {
//            var korisnik = await _userManager.FindByIdAsync(KupacId);
//            if (korisnik == null)
//            {
//                return NotFound("Korisnik nije pronađen.");
//            }

//            var favoriti = await _favoritiService.GetAllFavoritesByUserId(KupacId);
//            if (favoriti != null && favoriti.Any())
//            {
//                return Ok(favoriti);
//            }

//            return NotFound("Nema omiljenih umetničkih dela za ovog korisnika.");
//        }

//        // Dodavanje umetničkog dela u favorite
//        [HttpPost("DodajUFavorite/{UmetnickoDeloId}")]
//        //[Authorize(Roles = "Kupac")]
//        public async Task<IActionResult> DodajUFavoriti([FromRoute] int UmetnickoDeloId)
//        {
//            // Izvlačenje ID korisnika iz JWT tokena
//            var KupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(KupacId))
//            {
//                return Unauthorized(new { message = "Korisnik nije autorizovan." });
//            }

//            try
//            {
//                var favorit = await _favoritiService.AddToFavorites(KupacId, UmetnickoDeloId);
//                return Ok(favorit);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        // Uklanjanje umetničkog dela iz favorita
//        [HttpDelete("UkloniIzFavorita/{UmetnickoDeloId}")]
//        [Authorize(Roles = "Kupac")]
//        public async Task<IActionResult> UkloniIzFavorita(int UmetnickoDeloId)
//        {
//            // Izvlačenje ID korisnika iz JWT tokena
//            var KupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(KupacId))
//            {
//                return Unauthorized(new { message = "Korisnik nije autorizovan." });
//            }

//            var rezultat = await _favoritiService.RemoveFromFavorites(KupacId, UmetnickoDeloId);
//            if (rezultat)
//            {
//                return NoContent();
//            }

//            return NotFound("Umetničko delo nije pronađeno u favoritima.");
//        }
//    }
//}
