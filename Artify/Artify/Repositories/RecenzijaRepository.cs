using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.RecenzijaDTO;

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
            return await _context.Recenzije.Include(r => r.Kupac).Include(r => r.UmetnickoDelo).ToListAsync();
        }

        public async Task<Recenzija> GetRecenzijaByIdAsync(int RecenzijaId)
        {
            return await _context.Recenzije
                .Include(r => r.Kupac)
                .Include(r => r.UmetnickoDelo)
                .FirstOrDefaultAsync(r => r.RecenzijaId == RecenzijaId);
        }

        public async Task<IEnumerable<Recenzija>> GetRecenzijeZaUmetnickoDeloAsync(int UmetnickoDeloId)
        {
            return await _context.Recenzije
                .Where(r => r.UmetnickoDeloId == UmetnickoDeloId)
                .Include(r => r.Kupac)
                .ToListAsync();
        }

        public async Task<Recenzija> KreirajRecenzijuAsync(KreirajRecenzijuDTO NovaRecenzijaDTO)
        {
            var novaRecenzija = new Recenzija
            {
                KupacId = NovaRecenzijaDTO.KupacId,
                UmetnickoDeloId = NovaRecenzijaDTO.UmetnickoDeloId,
                Ocena = NovaRecenzijaDTO.Ocena,
                Komentar = NovaRecenzijaDTO.Komentar
            };

            _context.Recenzije.Add(novaRecenzija);
            await _context.SaveChangesAsync();
            return novaRecenzija;
        }

        public async Task<Recenzija> AzurirajRecenzijuAsync(AzurirajRecenzijuDTO IzmenjenaRecenzijaDTO)
        {
            var recenzija = await _context.Recenzije.FindAsync(IzmenjenaRecenzijaDTO.RecenzijaId);
            if (recenzija == null)
            {
                throw new ArgumentException("Recenzija sa zadatim ID-om ne postoji.");
            }

            if (IzmenjenaRecenzijaDTO.NovaOcena.HasValue)
            {
                recenzija.Ocena = IzmenjenaRecenzijaDTO.NovaOcena.Value;
            }

            if (!string.IsNullOrEmpty(IzmenjenaRecenzijaDTO.NoviKomentar))
            {
                recenzija.Komentar = IzmenjenaRecenzijaDTO.NoviKomentar;
            }

            await _context.SaveChangesAsync();
            return recenzija;
        }

        public async Task<bool> ObrisiRecenzijuAsync(int RecenzijaId)
        {
            var recenzija = await _context.Recenzije.FindAsync(RecenzijaId);
            if (recenzija == null)
            {
                return false;
            }

            _context.Recenzije.Remove(recenzija);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
