using Artify.Interfaces;
using Artify.Models;
using Artify.Data;
using Artify.DTO_klase.PorudzbinaDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artify.Repositories
{
    public class PorudzbinaRepository : IPorudzbina
    {
        private readonly AppDbContext _context;

        public PorudzbinaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Porudzbina>> GetAllPorudzbineAsync()
        {
            return await _context.Porudzbine
                .Include(p => p.Korisnik)
                .Include(p => p.UmetnickoDelo)
                .ThenInclude(d => d.Umetnik)
                .ToListAsync();
        }

        public async Task<Porudzbina> GetPorudzbinaByIdAsync(int PorudzbinaId)
        {
            return await _context.Porudzbine
                .Include(p => p.Korisnik)
                .Include(p => p.UmetnickoDelo)
                .ThenInclude(d => d.Umetnik)
                .FirstOrDefaultAsync(p => p.PorudzbinaId == PorudzbinaId);
        }

        public async Task<Porudzbina> CreatePorudzbinaAsync(KreiranjePorudzbineDTO dto)
        {
            var delo = await _context.UmetnickaDela.FindAsync(dto.UmetnickoDeloId);
            if (delo == null) throw new Exception("Umetničko delo ne postoji.");

            if (delo.Status != UmetnickoDeloStatus.Dostupno)
                throw new Exception("Delo nije dostupno za kupovinu.");

            if (delo.NaAukciji)
                throw new Exception("Aukcijsko delo se ne može kupiti direktno.");


            var porudzbina = new Porudzbina
            {
                UmetnickoDeloId = delo.UmetnickoDeloId,
                KorisnikId = dto.KorisnikId.ToString(), // pretvoriti u string
                CenaUTrenutkuKupovine = delo.Cena ?? 0f,
                Status = PorudzbinaStatus.NaCekanju
            };

            delo.Status = UmetnickoDeloStatus.Prodato;

            _context.Porudzbine.Add(porudzbina);
            await _context.SaveChangesAsync();
            return porudzbina;
        }

        public async Task UpdatePorudzbinaAsync(AzuriranjePorudzbineDTO dto)
        {
            var porudzbina = await _context.Porudzbine.FindAsync(dto.PorudzbinaId);
            if (porudzbina == null) throw new Exception("Porudžbina nije pronađena.");

            porudzbina.Status = dto.NoviStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePorudzbinaAsync(int PorudzbinaId)
        {
            var porudzbina = await _context.Porudzbine.FindAsync(PorudzbinaId);
            if (porudzbina == null) return false;

            var delo = await _context.UmetnickaDela.FindAsync(porudzbina.UmetnickoDeloId);
            if (delo != null && porudzbina.Status != PorudzbinaStatus.Placena)
                delo.Status = UmetnickoDeloStatus.Dostupno;

            _context.Porudzbine.Remove(porudzbina);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
