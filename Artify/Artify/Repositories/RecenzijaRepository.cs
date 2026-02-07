using Artify.Interfaces;
using Artify.Models;
using Artify.Data;
using Artify.DTO_klase.RecenzijaDTO;
using Microsoft.EntityFrameworkCore;

namespace Artify.Repositories
{
    public class RecenzijaRepository : IRecenzija
    {
        private readonly AppDbContext _context;

        public RecenzijaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recenzija>> GetSveRecenzijeAsync()
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .Include(r => r.UmetnickoDelo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Recenzija> GetRecenzijaByIdAsync(int RecenzijaId)
        {
            return await _context.Recenzije
                .Include(r => r.Korisnik)
                .Include(r => r.UmetnickoDelo)
                .FirstOrDefaultAsync(r => r.RecenzijaId == RecenzijaId);
        }

        public async Task<IEnumerable<Recenzija>> GetRecenzijeZaUmetnickoDeloAsync(int UmetnickoDeloId)
        {
            return await _context.Recenzije
                .Where(r => r.UmetnickoDeloId == UmetnickoDeloId)
                .Include(r => r.Korisnik)
                .AsNoTracking()
                .OrderByDescending(r => r.RecenzijaId)
                .ToListAsync();
        }

        public async Task<Recenzija> KreirajRecenzijuAsync(KreirajRecenzijuDTO dto)
        {
            var recenzija = new Recenzija
            {
                KorisnikId = dto.KorisnikId,
                UmetnickoDeloId = dto.UmetnickoDeloId,
                Ocena = dto.Ocena,
                Komentar = dto.Komentar
            };

            _context.Recenzije.Add(recenzija);
            await _context.SaveChangesAsync();

            return recenzija;
        }

        public async Task<Recenzija> AzurirajRecenzijuAsync(AzurirajRecenzijuDTO dto)
        {
            var recenzija = await _context.Recenzije
                .FirstOrDefaultAsync(r => r.RecenzijaId == dto.RecenzijaId);

            if (recenzija == null)
                throw new ArgumentException("Recenzija sa zadatim ID-om ne postoji.");

            recenzija.Ocena = dto.Ocena;       // int → int (nema nullable greške)
            recenzija.Komentar = dto.Komentar;

            await _context.SaveChangesAsync();
            return recenzija;
        }

        public async Task<bool> ObrisiRecenzijuAsync(int RecenzijaId)
        {
            var recenzija = await _context.Recenzije
                .FirstOrDefaultAsync(r => r.RecenzijaId == RecenzijaId);

            if (recenzija == null)
                return false;

            _context.Recenzije.Remove(recenzija);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
