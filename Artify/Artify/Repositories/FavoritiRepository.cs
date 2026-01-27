using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;

namespace Artify.Repositories
{
    public class FavoritiRepository : IFavoriti
    {
        private readonly AppDbContext _context;

        public FavoritiRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Favoriti>> GetAllFavoritesByUserId(string KupacId)
        {
            return await _context.Favoriti
                .Where(f => f.KupacId == KupacId)
                .Include(f => f.UmetnickoDelo) // Uključuje povezano umetničko delo
                .ToListAsync();
        }

        public async Task<Favoriti> AddToFavorites(string KupacId, int UmetnickoDeloId)
        {
            // Provera da li već postoji u omiljenim
            var existing = await _context.Favoriti
                .FirstOrDefaultAsync(f => f.KupacId == KupacId && f.UmetnickoDeloId == UmetnickoDeloId);

            if (existing != null)
            {
                throw new InvalidOperationException("Umetničko delo je već u omiljenim.");
            }

            var favoriti = new Favoriti
            {
                KupacId = KupacId,
                UmetnickoDeloId = UmetnickoDeloId
            };

            _context.Favoriti.Add(favoriti);
            await _context.SaveChangesAsync();

            return favoriti;
        }

        public async Task<bool> RemoveFromFavorites(string KupacId, int UmetnickoDeloId)
        {
            var favoriti = await _context.Favoriti
                .FirstOrDefaultAsync(f => f.KupacId == KupacId && f.UmetnickoDeloId == UmetnickoDeloId);

            if (favoriti == null)
            {
                return false; // Nije pronađen zapis
            }

            _context.Favoriti.Remove(favoriti);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
