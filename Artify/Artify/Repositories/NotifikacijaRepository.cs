using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.NotifikacijaDTO;

namespace Artify.Repositories
{
    public class NotifikacijaRepository : INotifikacija
    {
        private readonly AppDbContext _context;

        public NotifikacijaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notifikacija>> GetAllNotificationsAsync(string KorisnikId)
        {
            return await _context.Notifikacije
                .Include(n => n.Korisnik)
                .Where(n => n.KorisnikId == KorisnikId)
                .ToListAsync();
        }

        public async Task<Notifikacija> GetNotificationByIdAsync(int NotifikacijaId)
        {
            return await _context.Notifikacije
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(n => n.NotifikacijaId == NotifikacijaId);
        }

        public async Task<Notifikacija> CreateNotificationAsync(KreirajNotifikacijuDTO novaNotifikacija)
        {
            var notifikacija = new Notifikacija
            {
                KorisnikId = novaNotifikacija.KorisnikId,
                Poruka = novaNotifikacija.Poruka,
                Tip = novaNotifikacija.Tip,
                PorudzbinaId = novaNotifikacija.PorudzbinaId,
                UmetnickoDeloId = novaNotifikacija.UmetnickoDeloId
            };

            _context.Notifikacije.Add(notifikacija);
            await _context.SaveChangesAsync();
            return notifikacija;
        }

        public async Task<bool> UpdateNotificationAsync(AzurirajNotifikacijuDTO izmenaNotifikacije)
        {
            var notifikacija = await _context.Notifikacije.FindAsync(izmenaNotifikacije.NotifikacijaId);
            if (notifikacija == null) return false;

            if (!string.IsNullOrEmpty(izmenaNotifikacije.Poruka))
                notifikacija.Poruka = izmenaNotifikacije.Poruka;

            if (izmenaNotifikacije.Tip.HasValue)
                notifikacija.Tip = izmenaNotifikacije.Tip.Value;

            _context.Notifikacije.Update(notifikacija);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int NotifikacijaId)
        {
            var notifikacija = await _context.Notifikacije.FindAsync(NotifikacijaId);
            if (notifikacija == null) return false;

            _context.Notifikacije.Remove(notifikacija);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
