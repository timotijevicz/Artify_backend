using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.PortfolioDTO;

namespace Artify.Repositories
{
    public class PortfolioRepository : IPortfolio
    {
        private readonly AppDbContext _context;

        public PortfolioRepository(AppDbContext context)
        {
            _context = context;
        }

        // Dohvati sve portfolije
        public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync()
        {
            return await _context.Portfolio.Include(p => p.UmetnickaDela).ToListAsync();
        }

        // Dohvati portfolio po ID-u
        public async Task<Portfolio> GetPortfolioByIdAsync(int PortfolioId)
        {
            return await _context.Portfolio
                .Include(p => p.UmetnickaDela)
                .FirstOrDefaultAsync(p => p.PortfolioId == PortfolioId);
        }

        // Dohvati portfolije po ID-u korisnika
        public async Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(string KorisnikId)
        {
            return await _context.Portfolio
                .Where(p => p.UmetnikId == KorisnikId)
                .Include(p => p.UmetnickaDela)
                .ToListAsync();
        }

        // Kreiranje novog portfolia
        public async Task<Portfolio> CreatePortfolioAsync(KreiranjePortfoliaDTO dto)
        {
            var portfolio = new Portfolio
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                UmetnikId = dto.UmetnikId,
                UmetnickaDela = await _context.UmetnickaDela
                    .Where(d => dto.UmetnickaDelaId.Contains(d.UmetnickoDeloId))
                    .ToListAsync()
            };

            _context.Portfolio.Add(portfolio);
            await _context.SaveChangesAsync();

            return portfolio;
        }

        // Ažuriranje postojećeg portfolia
        public async Task<bool> UpdatePortfolioAsync(int PortfolioId, AzuriranjePortfoliaDTO dto)
        {
            var portfolio = await _context.Portfolio.FindAsync(PortfolioId);

            if (portfolio == null)
            {
                return false;  // Portfolio nije pronađen
            }

            portfolio.Naziv = dto.Naziv;
            portfolio.Opis = dto.Opis;
            portfolio.UmetnickaDela = await _context.UmetnickaDela
                .Where(d => dto.UmetnickaDelaId.Contains(d.UmetnickoDeloId))
                .ToListAsync();

            _context.Portfolio.Update(portfolio);
            await _context.SaveChangesAsync();

            return true;  // Portfolio uspešno ažuriran
        }

        // Brisanje portfolia
        public async Task<bool> DeletePortfolioAsync(int PortfolioId, string UmetnikId)
        {
            var portfolio = await _context.Portfolio
                .Where(p => p.PortfolioId == PortfolioId && p.UmetnikId == UmetnikId)
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                return false; // Portfolio nije pronađen ili ne pripada umetniku
            }

            _context.Portfolio.Remove(portfolio);
            await _context.SaveChangesAsync();

            return true; // Portfolio uspešno obrisan
        }
    }
}
