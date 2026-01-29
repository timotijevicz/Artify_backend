//using Artify.Interfaces;
//using Artify.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Artify.Repositories;
//using System.Security.Claims;
//using Artify.DTO_klase.KorisnikDTO;


//namespace Artify.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class KorisnikController : ControllerBase
//    {
//        private readonly IKorisnik _korisnikService;

//        public KorisnikController(IKorisnik korisnikService)
//        {
//            _korisnikService = korisnikService;
//        }

//        // Dohvati sve korisnike
//        [HttpGet ("DohvatiSveKorisnike")]
//        public async Task<IActionResult> GetAllUsers()
//        {
//            var korisnici = await _korisnikService.GetAllUsersAsync();
//            return Ok(korisnici);
//        }

//        // Dohvati korisnika po ID-u
//        [HttpGet("DohvatiKorisnikaPoID/{KorisnikId}")]
//        public async Task<IActionResult> GetUserById(string KorisnikId)
//        {
//            var korisnik = await _korisnikService.GetUserByIdAsync(KorisnikId);
//            if (korisnik == null)
//            {
//                return NotFound("Korisnik nije pronađen.");
//            }
//            return Ok(korisnik);
//        }

//        // Registracija korisnika
//        [HttpPost("RegistracijaKorisnika")]
//        public async Task<IActionResult> RegisterUser([FromBody] RegistracijaKorisnikaDTO registracijaDTO)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var result = await _korisnikService.RegisterAsync(registracijaDTO);
//            if (result == "Registracija uspešna.")
//            {
//                return Ok(result);
//            }

//            return BadRequest(result);
//        }

//        // Prijava korisnika
//        [HttpPost("PrijavaKorisnika")]
//        public async Task<IActionResult> LoginUser([FromBody] LogovanjeKorisnikaDTO logovanjeDTO)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var result = await _korisnikService.LoginAsync(logovanjeDTO);
//            if (result == "Prijava uspešna.")
//            {
//                return Ok(result);
//            }

//            return Unauthorized(result);
//        }

//        // Promena lozinke
//        [HttpPost("PromenaLozinke")]
//        public async Task<IActionResult> ChangePassword([FromBody] PromenaLozinkeKorisnikaDTO promenaLozinkeDTO)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var result = await _korisnikService.ChangePasswordAsync(promenaLozinkeDTO);
//            if (result == "Lozinka uspešno promenjena.")
//            {
//                return Ok(result);
//            }

//            return BadRequest(result);
//        }

//        // Brisanje korisnika
//        [HttpDelete("BrisanjeKorisnika/{KorisnikId}")]
//        public async Task<IActionResult> DeleteUser(string id)
//        {
//            try
//            {
//                await _korisnikService.DeleteUserAsync(id);
//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }

//        // Brisanje umetnika i njegovih umetničkih dela
//        [HttpDelete("BrisanjeUmetnika/{id}")]
//        public async Task<IActionResult> DeleteArtist(string id, [FromBody] IEnumerable<int> umetnickaDelaIds)
//        {
//            try
//            {
//                await _korisnikService.DeleteArtistAsync(id, umetnickaDelaIds);
//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }

//        // Odjava korisnika
//        [HttpPost("Odjava")]
//        public async Task<IActionResult> Logout()
//        {
//            await _korisnikService.LogoutAsync();
//            return Ok("Odjava uspešna.");
//        }
//    }
//}
