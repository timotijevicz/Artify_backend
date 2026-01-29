using Artify.Interfaces;
using Artify.Models;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artify.Repositories
{
    public class FavoritiRepository : IFavoriti
    {
        private readonly AppDbContext _context;

        public FavoritiRepository(AppDbContext context)
        {
            _context = context;
        }

        // Vraća sve favoriti za određenog korisnika
        public async Task<List<Favoriti>> GetAllFavoritesByUserId(string KorisnikId)
        {
            return await _context.Favoriti
                .Where(f => f.KorisnikId == KorisnikId)
                .Include(f => f.UmetnickoDelo)
                .ToListAsync();
        }

        // Dodaje umetničko delo u omiljene
        public async Task<Favoriti> AddToFavorites(string KorisnikId, int umetnickoDeloId)
        {
            // Provera da li već postoji u omiljenim
            var existing = await _context.Favoriti
                .FirstOrDefaultAsync(f => f.KorisnikId == KorisnikId && f.UmetnickoDeloId == umetnickoDeloId);

            if (existing != null)
                throw new InvalidOperationException("Umetničko delo je već u omiljenim.");

            var favoriti = new Favoriti
            {
                KorisnikId = KorisnikId,
                UmetnickoDeloId = umetnickoDeloId
            };

            _context.Favoriti.Add(favoriti);
            await _context.SaveChangesAsync();

            return favoriti;
        }

        // Uklanja umetničko delo iz omiljenih
        public async Task<bool> RemoveFromFavorites(string KorisnikId, int umetnickoDeloId)
        {
            var favoriti = await _context.Favoriti
                .FirstOrDefaultAsync(f => f.KorisnikId == KorisnikId && f.UmetnickoDeloId == umetnickoDeloId);

            if (favoriti == null)
                return false;

            _context.Favoriti.Remove(favoriti);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
