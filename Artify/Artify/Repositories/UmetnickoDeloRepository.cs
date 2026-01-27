using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Artify.DTO_klase.UmetnickoDeloDTO;

namespace Artify.Repositories
{
    public class UmetnickoDeloRepository : IUmetnickoDelo
    {
        private readonly AppDbContext _context;

        public UmetnickoDeloRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<UmetnickoDelo>> GetAllArtworksAsync()
        {
            return await _context.UmetnickaDela.Include(d => d.Kategorija).Include(d => d.Umetnik).ToListAsync();
        }

        public async Task<UmetnickoDelo> GetArtworkByIdAsync(int UmetnickoDeloId)
        {
            return await _context.UmetnickaDela.Include(d => d.Kategorija).Include(d => d.Umetnik)
                                               .FirstOrDefaultAsync(d => d.UmetnickoDeloId == UmetnickoDeloId);
        }

        public async Task<UmetnickoDelo> AddArtworkAsync(KreirajUmetnickoDeloDTO NovoDeloDTO)
        {
            var novoDelo = new UmetnickoDelo
            {
                Naziv = NovoDeloDTO.Naziv,
                Opis = NovoDeloDTO.Opis,
                Cena = NovoDeloDTO.Cena,
                Slika = NovoDeloDTO.Slika,
                Tehnika = NovoDeloDTO.Tehnika,
                KategorijaId = NovoDeloDTO.KategorijaId,
                Stil = NovoDeloDTO.Stil,
                Dimenzije = NovoDeloDTO.Dimenzije,
                UmetnikId = NovoDeloDTO.UmetnikId,
                Status = UmetnickoDeloStatus.Dostupno
            };

            _context.UmetnickaDela.Add(novoDelo);
            await _context.SaveChangesAsync();
            return novoDelo;
        }

        public async Task<bool> UpdateArtworkAsync(AzuriranjeUmetnickogDelaDTO IzmenjenoDeloDTO)
        {
            var delo = await _context.UmetnickaDela.FindAsync(IzmenjenoDeloDTO.UmetnickoDeloId);
            if (delo == null) return false;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Naziv))
                delo.Naziv = IzmenjenoDeloDTO.Naziv;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Opis))
                delo.Opis = IzmenjenoDeloDTO.Opis;

            if (IzmenjenoDeloDTO.Cena.HasValue)
                delo.Cena = IzmenjenoDeloDTO.Cena.Value;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Slika))
                delo.Slika = IzmenjenoDeloDTO.Slika;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Tehnika))
                delo.Tehnika = IzmenjenoDeloDTO.Tehnika;

            if (IzmenjenoDeloDTO.KategorijaId.HasValue)
                delo.KategorijaId = IzmenjenoDeloDTO.KategorijaId;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Stil))
                delo.Stil = IzmenjenoDeloDTO.Stil;

            if (!string.IsNullOrEmpty(IzmenjenoDeloDTO.Dimenzije))
                delo.Dimenzije = IzmenjenoDeloDTO.Dimenzije;

            if (IzmenjenoDeloDTO.Status.HasValue)
                delo.Status = IzmenjenoDeloDTO.Status.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteArtworkAsync(int id)
        {
            var delo = await _context.UmetnickaDela.FindAsync(id);
            if (delo == null) return false;

            _context.UmetnickaDela.Remove(delo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UmetnickoDelo>> GetArtworksByCategoryAsync(int KategorijaId)
        {
            return await _context.UmetnickaDela.Where(d => d.KategorijaId == KategorijaId)
                                               .Include(d => d.Kategorija)
                                               .ToListAsync();
        }

        public async Task<IEnumerable<UmetnickoDelo>> GetArtworksByArtistAsync(string UmetnikId)
        {
            return await _context.UmetnickaDela.Where(d => d.UmetnikId == UmetnikId)
                                               .Include(d => d.Umetnik)
                                               .ToListAsync();
        }

        public async Task<IEnumerable<UmetnickoDelo>> SearchArtworksAsync(string keyword)
        {
            return await _context.UmetnickaDela
                .Where(d => d.Naziv.Contains(keyword) || d.Opis.Contains(keyword))
                .Include(d => d.Kategorija)
                .Include(d => d.Umetnik)
                .ToListAsync();
        }
    }
}
