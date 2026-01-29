using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.UmetnikDTO;

namespace Artify.Repositories
{
    public class UmetnikRepository : IUmetnik
    {
        private readonly AppDbContext _context;

        public UmetnikRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Umetnik>> GetAllArtistsAsync()
        {
            return await _context.Umetnici
                .Include(u => u.Korisnik)
                .Include(u => u.UmetnickaDela)
                .ToListAsync();
        }

        public async Task<Umetnik> GetArtistByIdAsync(int UmetnikId)
        {
            return await _context.Umetnici
                .Include(u => u.Korisnik)
                .Include(u => u.UmetnickaDela)
                .FirstOrDefaultAsync(u => u.UmetnikId == UmetnikId);
        }

        public async Task<Umetnik> CreateArtistAsync(KreirajUmetnikaDTO noviUmetnik)
        {
            var umetnik = new Umetnik
            {
                KorisnikId = noviUmetnik.KorisnikId,
                Biografija = noviUmetnik.Biografija,
                Tehnika = noviUmetnik.Tehnika,
                Stil = noviUmetnik.Stil,
                Specijalizacija = noviUmetnik.Specijalizacija,
                SlikaUrl = noviUmetnik.SlikaUrl,
                IsApproved = false,
                IsAvailable = false,
            };

            _context.Umetnici.Add(umetnik);
            await _context.SaveChangesAsync();
            return umetnik;
        }

        public async Task<bool> UpdateArtistAsync(AzurirajUmetnikaDTO izmenaUmetnika)
        {
            var umetnik = await _context.Umetnici.FindAsync(izmenaUmetnika.UmetnikId);
            if (umetnik == null) return false;

            if (!string.IsNullOrEmpty(izmenaUmetnika.Biografija))
                umetnik.Biografija = izmenaUmetnika.Biografija;

            if (!string.IsNullOrEmpty(izmenaUmetnika.Tehnika))
                umetnik.Tehnika = izmenaUmetnika.Tehnika;

            if (!string.IsNullOrEmpty(izmenaUmetnika.Stil))
                umetnik.Stil = izmenaUmetnika.Stil;

            if (!string.IsNullOrEmpty(izmenaUmetnika.Specijalizacija))
                umetnik.Specijalizacija = izmenaUmetnika.Specijalizacija;

            if (!string.IsNullOrEmpty(izmenaUmetnika.SlikaUrl))
                umetnik.SlikaUrl = izmenaUmetnika.SlikaUrl;

            if (izmenaUmetnika.IsApproved.HasValue)
                umetnik.IsApproved = izmenaUmetnika.IsApproved.Value;

            if (izmenaUmetnika.IsAvailable.HasValue)
                umetnik.IsAvailable = izmenaUmetnika.IsAvailable.Value;

            _context.Umetnici.Update(umetnik);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteArtistAsync(int UmetnikId)
        {
            var umetnik = await _context.Umetnici.FindAsync(UmetnikId);
            if (umetnik == null) return false;

            _context.Umetnici.Remove(umetnik);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveArtistAsync(int UmetnikId)
        {
            var umetnik = await _context.Umetnici.FindAsync(UmetnikId);
            if (umetnik == null) return false;

            umetnik.IsApproved = true;
            umetnik.IsAvailable = true;

            _context.Umetnici.Update(umetnik);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
