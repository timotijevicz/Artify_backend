using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Artify.Data;
using Artify.DTO_klase.AukcijskaPonudaDTO;
using Artify.Models;

namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AukcijaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AukcijaController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/Aukcija/Ponude/20
        [HttpGet("Ponude/{umetnickoDeloId:int}")]
        public async Task<ActionResult<List<PonudaDTO>>> Ponude(int umetnickoDeloId)
        {
            var exists = await _db.UmetnickaDela
                .AsNoTracking()
                .AnyAsync(d => d.UmetnickoDeloId == umetnickoDeloId);

            if (!exists) return NotFound("Delo ne postoji.");

            // ✅ Identity korisnici su u _db.Users (IdentityDbContext)
            var list = await (
                from p in _db.AukcijskePonude.AsNoTracking()
                join u in _db.Users.AsNoTracking() on p.KupacId equals u.Id into uu
                from u in uu.DefaultIfEmpty()
                where p.UmetnickoDeloId == umetnickoDeloId
                orderby p.Iznos descending, p.DatumKreiranja descending
                select new PonudaDTO
                {
                    PonudaId = p.AukcijskaPonudaId,
                    UmetnickoDeloId = p.UmetnickoDeloId,
                    KupacId = p.KupacId,
                    Iznos = p.Iznos,
                    DatumKreiranja = p.DatumKreiranja,
                    KupacIme = (u != null ? (u.ImeIPrezime ?? u.UserName ?? u.Email) : null)
                }
            ).ToListAsync();

            return Ok(list);
        }

        // POST: /api/Aukcija/DodajPonudu
        [Authorize(Roles = "Kupac")]
        [HttpPost("DodajPonudu")]
        public async Task<ActionResult<PonudaDTO>> DodajPonudu([FromBody] DodajPonuduDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Iznos == null)
                return BadRequest("Iznos je obavezan.");

            float iznos = dto.Iznos.Value;

            var kupacId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(kupacId))
                return Unauthorized("Nema korisničkog identiteta u tokenu.");

            await using var tx = await _db.Database.BeginTransactionAsync();

            var delo = await _db.UmetnickaDela
                .FirstOrDefaultAsync(d => d.UmetnickoDeloId == dto.UmetnickoDeloId);

            if (delo == null) return NotFound("Delo ne postoji.");
            if (!delo.NaAukciji) return BadRequest("Delo nije na aukciji.");

            var now = DateTime.UtcNow;

            if (delo.AukcijaPocinje.HasValue && now < delo.AukcijaPocinje.Value)
                return BadRequest("Aukcija još nije počela.");

            if (delo.AukcijaZavrsava.HasValue && now > delo.AukcijaZavrsava.Value)
                return BadRequest("Aukcija je završena.");

            // max ponuda iz baze (float-safe)
            float? maxBid = await _db.AukcijskePonude
                .Where(p => p.UmetnickoDeloId == dto.UmetnickoDeloId)
                .MaxAsync(p => (float?)p.Iznos);

            float trenutna = maxBid
                ?? (delo.TrenutnaCenaAukcije ?? delo.PocetnaCenaAukcije ?? 0f);

            if (iznos <= trenutna)
                return BadRequest($"Ponuda mora biti veća od trenutne ({trenutna}).");

            var ponuda = new AukcijskaPonuda
            {
                UmetnickoDeloId = dto.UmetnickoDeloId,
                KupacId = kupacId,
                Iznos = iznos,
                DatumKreiranja = DateTime.UtcNow
            };

            _db.AukcijskePonude.Add(ponuda);

            // update trenutno stanje
            delo.TrenutnaCenaAukcije = iznos;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            // ✅ uzmi kupca iz _db.Users
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == kupacId);

            var outDto = new PonudaDTO
            {
                PonudaId = ponuda.AukcijskaPonudaId,
                UmetnickoDeloId = ponuda.UmetnickoDeloId,
                KupacId = ponuda.KupacId,
                Iznos = ponuda.Iznos,
                DatumKreiranja = ponuda.DatumKreiranja,
                KupacIme = u?.ImeIPrezime ?? u?.UserName ?? u?.Email
            };

            return Ok(outDto);
        }

        // POST: /api/Aukcija/Finalize
        // ✅ Preporuka: zaštiti endpoint (po potrebi promeni role)
        // [Authorize(Roles = "Admin,Umetnik")]
        [HttpPost("Finalize")]
        public async Task<IActionResult> Finalize([FromBody] FinalizeAukcijaDTO dto)
        {
            if (dto == null || dto.UmetnickoDeloId <= 0)
                return BadRequest("Neispravan zahtev.");

            await using var tx = await _db.Database.BeginTransactionAsync();

            var delo = await _db.UmetnickaDela
                .FirstOrDefaultAsync(d => d.UmetnickoDeloId == dto.UmetnickoDeloId);

            if (delo == null) return NotFound("Delo ne postoji.");

            // Ako je već finalizovano (NaAukciji == false), vrati stanje "koliko možeš"
            if (!delo.NaAukciji)
            {
                var existingOrder = await _db.Porudzbine
                    .AsNoTracking()
                    .Where(x => x.UmetnickoDeloId == dto.UmetnickoDeloId)
                    .Select(x => new { x.PorudzbinaId, x.KorisnikId, x.CenaUTrenutkuKupovine })
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    poruka = "Aukcija je već finalizovana.",
                    pobednikKorisnikId = existingOrder?.KorisnikId,
                    PobednikKorisnikId = existingOrder?.KorisnikId,
                    iznos = existingOrder?.CenaUTrenutkuKupovine,
                    porudzbinaId = existingOrder?.PorudzbinaId
                });
            }

            var now = DateTime.UtcNow;

            if (!delo.AukcijaZavrsava.HasValue)
                return BadRequest("Aukcija nema datum završetka.");

            if (now < delo.AukcijaZavrsava.Value)
                return BadRequest("Aukcija još uvek traje.");

            // Uzmi top ponudu
            var top = await _db.AukcijskePonude
                .Where(p => p.UmetnickoDeloId == dto.UmetnickoDeloId && p.Iznos != null)
                .OrderByDescending(p => p.Iznos)
                .ThenByDescending(p => p.DatumKreiranja)
                .Select(p => new { p.KupacId, p.Iznos })
                .FirstOrDefaultAsync();

            // Ugasi aukciju u svakom slučaju
            delo.NaAukciji = false;

            // NEMA PONUDA -> delo ostaje dostupno (nije prodato)
            if (top == null || top.Iznos == null)
            {
                delo.TrenutnaCenaAukcije = null;
                delo.Status = UmetnickoDeloStatus.Dostupno;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    poruka = "Aukcija završena bez ponuda.",
                    pobednikKorisnikId = (string?)null,
                    PobednikKorisnikId = (string?)null,
                    porudzbinaId = (int?)null
                });
            }

            // IMA POBEDNIKA -> prodato
            var winnerId = top.KupacId;
            var winAmount = top.Iznos.Value;

            // probaj da nađeš postojeću porudžbinu
            var existingId = await _db.Porudzbine
                .Where(x => x.UmetnickoDeloId == dto.UmetnickoDeloId)
                .Select(x => (int?)x.PorudzbinaId)
                .FirstOrDefaultAsync();

            Porudzbina? created = null;

            if (existingId == null)
            {
                created = new Porudzbina
                {
                    UmetnickoDeloId = dto.UmetnickoDeloId,
                    KorisnikId = winnerId,
                    CenaUTrenutkuKupovine = winAmount,
                    Status = PorudzbinaStatus.NaCekanju,
                    DatumKreiranja = DateTime.UtcNow
                };

                _db.Porudzbine.Add(created);
            }

            delo.TrenutnaCenaAukcije = winAmount;
            delo.Status = UmetnickoDeloStatus.Prodato;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var porudzbinaId = created?.PorudzbinaId ?? existingId;

            return Ok(new
            {
                poruka = "Aukcija finalizovana. Porudžbina kreirana pobedniku.",
                pobednikKorisnikId = winnerId,
                PobednikKorisnikId = winnerId,
                iznos = winAmount,
                porudzbinaId = porudzbinaId
            });
        }
    }
}
