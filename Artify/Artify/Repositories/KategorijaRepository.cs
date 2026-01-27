using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase;
using Artify.DTO_klase.KategorijeDTO;


namespace Artify.Repositories
{

    public class KategorijaService : IKategorija
    {
        private readonly AppDbContext _context;

        public KategorijaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Kategorija>> GetAllKategorijeAsync()
        {
            return await _context.Kategorija.ToListAsync();
        }

        public async Task<Kategorija> GetKategorijaByIdAsync(int KategorijaId)
        {
            return await _context.Kategorija.FindAsync(KategorijaId);
        }

        public async Task<Kategorija> CreateKategorijaAsync(KreiranjeKategorijeDTO NovaKategorijaDTO)
        {
            var novaKategorija = new Kategorija
            {
                Naziv = NovaKategorijaDTO.Naziv
            };

            _context.Kategorija.Add(novaKategorija);
            await _context.SaveChangesAsync();

            return novaKategorija;
        }

        public async Task<Kategorija> UpdateKategorijaAsync(AzuriranjeKategorijeDTO IzmenjenaKategorijaDTO)
        {
            var kategorija = await _context.Kategorija.FindAsync(IzmenjenaKategorijaDTO.KategorijaId);

            if (kategorija == null)
                return null;

            kategorija.Naziv = IzmenjenaKategorijaDTO.Naziv;

            await _context.SaveChangesAsync();

            return kategorija;
        }

        public async Task<bool> DeleteKategorijaAsync(int KategorijaId)
        {
            var kategorija = await _context.Kategorija.FindAsync(KategorijaId);

            if (kategorija == null)
                return false;

            _context.Kategorija.Remove(kategorija);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
