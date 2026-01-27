using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.PorudzbinaDTO;

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
            return await _context.Porudzbine.Include(p => p.KupljenaDela).ToListAsync();
        }

        public async Task<Porudzbina> GetPorudzbinaByIdAsync(int PorudzbinaId)
        {
            return await _context.Porudzbine.Include(p => p.KupljenaDela).FirstOrDefaultAsync(p => p.PorudzbinaId == PorudzbinaId);
        }

        public async Task<Porudzbina> CreatePorudzbinaAsync(KreiranjePorudzbineDTO NovaPorudzbinaDTO)
        {
            var novaPorudzbina = new Porudzbina
            {
                UkupnaCena = NovaPorudzbinaDTO.UkupnaCena,
                KorpaId = NovaPorudzbinaDTO.KorpaId,
                KupljenaDela = await _context.UmetnickaDela
                    .Where(d => NovaPorudzbinaDTO.KupljenaDelaId.Contains(d.UmetnickoDeloId))
                    .ToListAsync()
            };

            _context.Porudzbine.Add(novaPorudzbina);
            await _context.SaveChangesAsync();
            return novaPorudzbina;
        }

        public async Task UpdatePorudzbinaAsync(AzuriranjePorudzbineDTO IzmenaPorudzbineDTO)
        {
            var porudzbina = await _context.Porudzbine.Include(p => p.KupljenaDela).FirstOrDefaultAsync(p => p.PorudzbinaId == IzmenaPorudzbineDTO.PorudzbinaId);
            if (porudzbina == null) throw new Exception("Porudžbina nije pronađena.");

            if (IzmenaPorudzbineDTO.NovaUkupnaCena.HasValue)
            {
                porudzbina.UkupnaCena = IzmenaPorudzbineDTO.NovaUkupnaCena.Value;
            }

            if (IzmenaPorudzbineDTO.NoviStatus.HasValue)
            {
                porudzbina.Status = IzmenaPorudzbineDTO.NoviStatus.Value;
            }

            if (IzmenaPorudzbineDTO.NovaKupljenaDelaId != null)
            {
                porudzbina.KupljenaDela = await _context.UmetnickaDela
                    .Where(d => IzmenaPorudzbineDTO.NovaKupljenaDelaId.Contains(d.UmetnickoDeloId))
                    .ToListAsync();
            }

            _context.Porudzbine.Update(porudzbina);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePorudzbinaAsync(int PorudzbinaId)
        {
            var porudzbina = await _context.Porudzbine.FindAsync(PorudzbinaId);
            if (porudzbina == null) return false;

            _context.Porudzbine.Remove(porudzbina);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
