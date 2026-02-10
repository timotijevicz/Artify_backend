using Artify.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Artify.DTO_klase.KorisnikDTO;
using Artify.DTO_klase.UmetnikDTO;
using System.Security.Claims;

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

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [Authorize(Roles = "Admin")]
        [HttpGet("DohvatiSveKorisnike")]
        public async Task<IActionResult> GetAllUsers()
        {
            var korisnici = await _korisnikService.GetAllUsersAsync();
            return Ok(korisnici);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("BrisanjeUmetnika/{korisnikId}")]
        public async Task<IActionResult> DeleteArtist(string korisnikId, [FromBody] IEnumerable<int> umetnickaDelaIds)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
                return BadRequest("ID umetnika ne sme biti prazan.");

            await _korisnikService.DeleteArtistAsync(korisnikId, umetnickaDelaIds ?? Enumerable.Empty<int>());
            return NoContent();
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("RegistracijaUmetnika")]
        public async Task<IActionResult> RegisterArtist([FromBody] RegistracijaUmetnikaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _korisnikService.RegisterArtistAsync(dto);

            if (result == "Registracija umetnika uspešna.")
                return Ok(result);

            return BadRequest(result);
        }

        [AllowAnonymous]
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

        [Authorize]
        [HttpPost("Odjava")]
        public async Task<IActionResult> Logout()
        {
            await _korisnikService.LogoutAsync();
            return Ok("Odjava uspešna.");
        }

        [Authorize]
        [HttpGet("MojProfil")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var profil = await _korisnikService.GetMyProfileAsync(userId);
            if (profil == null)
                return NotFound("Korisnik nije pronađen.");

            return Ok(profil);
        }

        [Authorize]
        [HttpPost("PromenaLozinkeMojProfil")]
        public async Task<IActionResult> ChangePasswordMyProfile([FromBody] PromenaLozinkeKorisnikaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _korisnikService.ChangePasswordMyAsync(userId, dto);

            if (result == "Lozinka uspešno promenjena.")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("PromenaEmailaMojProfil")]
        public async Task<IActionResult> ChangeEmailMyProfile([FromBody] PromenaEmailKorisnikaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _korisnikService.ChangeEmailMyAsync(userId, dto);

            if (result == "Email uspešno promenjen.")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("BrisanjeMogNaloga")]
        public async Task<IActionResult> DeleteMyAccount([FromBody] BrisanjeNalogaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _korisnikService.DeleteMyAccountAsync(userId, dto);

            if (result == "Nalog obrisan.")
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
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
    }
}
