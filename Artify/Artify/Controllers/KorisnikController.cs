using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Artify.DTO_klase.KorisnikDTO;
using Artify.DTO_klase.UmetnikDTO;

namespace Artify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly IKorisnik _korisnikService;

        public KorisnikController(IKorisnik korisnikService)
        {
            _korisnikService = korisnikService;
        }

        // Dohvati sve korisnike
        [HttpGet("DohvatiSveKorisnike")]
        public async Task<IActionResult> GetAllUsers()
        {
            var korisnici = await _korisnikService.GetAllUsersAsync();
            return Ok(korisnici);
        }

        // Dohvati korisnika po ID-u (string jer IdentityUser.Id je string)
        [HttpGet("DohvatiKorisnikaPoID/{korisnikId}")]
        public async Task<IActionResult> GetUserById(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
                return BadRequest("ID korisnika ne sme biti prazan.");

            var korisnik = await _korisnikService.GetUserByIdAsync(korisnikId);
            if (korisnik == null)
                return NotFound("Korisnik nije pronađen.");

            return Ok(korisnik);
        }

        // Registracija korisnika
        [HttpPost("RegistracijaKorisnika")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistracijaKorisnikaDTO registracijaDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _korisnikService.RegisterAsync(registracijaDTO);

            if (result == "Registracija uspešna.")
                return Ok(result);

            return BadRequest(result);
        }

        //registracija umetnika
        [HttpPost("RegistracijaUmetnika")]
        public async Task<IActionResult> RegisterArtist([FromBody] RegistracijaUmetnikaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _korisnikService.RegisterArtistAsync(dto);

            if (result == "Registracija umetnika uspešna.")
                return Ok(result);

            return BadRequest(result);
        }

        // Prijava korisnika
        [HttpPost("PrijavaKorisnika")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUser([FromBody] LogovanjeKorisnikaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Neispravni podaci." });

            var result = await _korisnikService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Pogrešan email ili lozinka." });

            return Ok(result);
        }

        // Promena lozinke
        [HttpPost("PromenaLozinke")]
        public async Task<IActionResult> ChangePassword([FromBody] PromenaLozinkeKorisnikaDTO promenaLozinkeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _korisnikService.ChangePasswordAsync(promenaLozinkeDTO);

            if (result == "Lozinka uspešno promenjena.")
                return Ok(result);

            return BadRequest(result);
        }

        // Brisanje korisnika
        [HttpDelete("BrisanjeKorisnika/{korisnikId}")]
        public async Task<IActionResult> DeleteUser(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
                return BadRequest("ID korisnika ne sme biti prazan.");

            var korisnik = await _korisnikService.GetUserByIdAsync(korisnikId);
            if (korisnik == null)
                return NotFound("Korisnik ne postoji.");

            await _korisnikService.DeleteUserAsync(korisnikId);
            return NoContent();
        }

        // Brisanje umetnika i njegovih umetničkih dela
        [HttpDelete("BrisanjeUmetnika/{korisnikId}")]
        public async Task<IActionResult> DeleteArtist(string korisnikId, [FromBody] IEnumerable<int> umetnickaDelaIds)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
                return BadRequest("ID umetnika ne sme biti prazan.");

            await _korisnikService.DeleteArtistAsync(korisnikId, umetnickaDelaIds ?? Enumerable.Empty<int>());
            return NoContent();
        }

        // Odjava korisnika
        [HttpPost("Odjava")]
        public async Task<IActionResult> Logout()
        {
            await _korisnikService.LogoutAsync();
            return Ok("Odjava uspešna.");
        }
    }
}
