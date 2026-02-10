using Artify.Data;
using Artify.DTO_klase.UmetnikDTO;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UmetnikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UmetnikController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // =========================================================
        // JAVNO / OPŠTE
        // =========================================================

        [HttpGet("SviUmetnici")]
        public async Task<IActionResult> GetAllArtists()
        {
            var umetnici = await _context.Umetnici
                .Include(u => u.Korisnik)
                .Include(u => u.UmetnickaDela)
                .ToListAsync();

            return Ok(umetnici);
        }

        [Authorize]
        [HttpGet("VracaUmetnikaPoID/{UmetnikId:int}")]
        public async Task<IActionResult> GetArtistById(int UmetnikId)
        {
            var umetnik = await _context.Umetnici
                .Include(u => u.Korisnik)
                .Include(u => u.UmetnickaDela)
                .FirstOrDefaultAsync(u => u.UmetnikId == UmetnikId);

            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            return Ok(umetnik);
        }

        // =========================================================
        // MOJ UMETNIK PROFIL (radi preko tokena i _context)
        // =========================================================

        [Authorize(Roles = "Umetnik")]
        [HttpGet("MojUmetnikProfil")]
        public async Task<IActionResult> MojUmetnikProfil()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var umetnik = await _context.Umetnici
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.KorisnikId == userId);

            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik profil nije pronađen." });

            return Ok(umetnik);
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPut("AzurirajMojUmetnikProfil")]
        public async Task<IActionResult> AzurirajMojUmetnikProfil([FromBody] AzurirajUmetnikaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var umetnik = await _context.Umetnici.FirstOrDefaultAsync(u => u.KorisnikId == userId);
            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik profil nije pronađen." });

            // update samo polja koja dolaze (null => ne diraj)
            if (dto.Biografija != null) umetnik.Biografija = dto.Biografija;
            if (dto.Tehnika != null) umetnik.Tehnika = dto.Tehnika;
            if (dto.Stil != null) umetnik.Stil = dto.Stil;
            if (dto.Specijalizacija != null) umetnik.Specijalizacija = dto.Specijalizacija;

            // Grad nije bio u tvom AzurirajUmetnikaDTO — ako želiš grad na "moj profil"
            // dodaj `public string? Grad {get;set;}` u DTO pa odkomentariši:
            // if (dto.Grad != null) umetnik.Grad = dto.Grad;

            if (dto.SlikaUrl != null) umetnik.SlikaUrl = dto.SlikaUrl;

            if (dto.IsAvailable.HasValue) umetnik.IsAvailable = dto.IsAvailable.Value;

            // IsApproved NE dozvoljavamo umetniku da menja
            // if (dto.IsApproved.HasValue) umetnik.IsApproved = dto.IsApproved.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class PostaviSlikuDTO
        {
            public string SlikaUrl { get; set; } = string.Empty;
        }

        [Authorize(Roles = "Umetnik")]
        [HttpPost("PostaviMojuSliku")]
        public async Task<IActionResult> PostaviMojuSliku([FromBody] PostaviSlikuDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.SlikaUrl))
                return BadRequest("SlikaUrl je obavezna.");

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var umetnik = await _context.Umetnici.FirstOrDefaultAsync(u => u.KorisnikId == userId);
            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik profil nije pronađen." });

            umetnik.SlikaUrl = dto.SlikaUrl;
            await _context.SaveChangesAsync();

            return Ok(new { Poruka = "Slika ažurirana.", SlikaUrl = dto.SlikaUrl });
        }

        // =========================================================
        // CRUD (kao tvoj stari)
        // =========================================================

        [HttpPost("KreirajUmetnika")]
        public async Task<IActionResult> CreateArtist([FromBody] KreirajUmetnikaDTO noviUmetnik)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var umetnik = new Umetnik
            {
                KorisnikId = noviUmetnik.KorisnikId,
                Biografija = noviUmetnik.Biografija,
                Tehnika = noviUmetnik.Tehnika,
                Stil = noviUmetnik.Stil,
                Specijalizacija = noviUmetnik.Specijalizacija,
                Grad = noviUmetnik.Grad,
                SlikaUrl = noviUmetnik.SlikaUrl,
                IsApproved = false,
                IsAvailable = false,
                DatumKreiranja = DateTime.UtcNow
            };

            _context.Umetnici.Add(umetnik);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArtistById), new { UmetnikId = umetnik.UmetnikId }, umetnik);
        }

        [HttpPut("AzurirajUmetnika")]
        public async Task<IActionResult> UpdateArtist([FromBody] AzurirajUmetnikaDTO izmenaUmetnika)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!izmenaUmetnika.UmetnikId.HasValue)
                return BadRequest("UmetnikId je obavezan.");

            var umetnik = await _context.Umetnici.FindAsync(izmenaUmetnika.UmetnikId.Value);
            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            if (izmenaUmetnika.Biografija != null) umetnik.Biografija = izmenaUmetnika.Biografija;
            if (izmenaUmetnika.Tehnika != null) umetnik.Tehnika = izmenaUmetnika.Tehnika;
            if (izmenaUmetnika.Stil != null) umetnik.Stil = izmenaUmetnika.Stil;
            if (izmenaUmetnika.Specijalizacija != null) umetnik.Specijalizacija = izmenaUmetnika.Specijalizacija;
            if (izmenaUmetnika.Grad != null) umetnik.Grad = izmenaUmetnika.Grad;
            if (izmenaUmetnika.SlikaUrl != null) umetnik.SlikaUrl = izmenaUmetnika.SlikaUrl;

            if (izmenaUmetnika.IsApproved.HasValue) umetnik.IsApproved = izmenaUmetnika.IsApproved.Value;
            if (izmenaUmetnika.IsAvailable.HasValue) umetnik.IsAvailable = izmenaUmetnika.IsAvailable.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("ObrisiUmetnika/{UmetnikId:int}")]
        public async Task<IActionResult> DeleteArtist(int UmetnikId)
        {
            var umetnik = await _context.Umetnici.FindAsync(UmetnikId);
            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            _context.Umetnici.Remove(umetnik);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("OdobriUmetnika/{UmetnikId:int}")]
        public async Task<IActionResult> ApproveArtist(int UmetnikId)
        {
            var umetnik = await _context.Umetnici.FindAsync(UmetnikId);
            if (umetnik == null)
                return NotFound(new { Poruka = "Umetnik nije pronađen." });

            umetnik.IsApproved = true;
            umetnik.IsAvailable = true;

            await _context.SaveChangesAsync();
            return Ok(new { Poruka = "Umetnik odobren i dostupan za prikaz." });
        }
    }
}
