using Artify.Interfaces;
using Artify.Models;
using Artify.Data;
using Artify.DTO_klase.UmetnickoDeloDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
            return await _context.UmetnickaDela
                .Include(d => d.Umetnik)
                .ToListAsync();
        }

        public async Task<UmetnickoDelo> GetArtworkByIdAsync(int umetnickoDeloId)
        {
            return await _context.UmetnickaDela
                .Include(d => d.Umetnik)
                .FirstOrDefaultAsync(d => d.UmetnickoDeloId == umetnickoDeloId);
        }

        public async Task<UmetnickoDelo> AddArtworkAsync(KreirajUmetnickoDeloDTO dto)
        {
            // VALIDACIJA – FIKSNA PRODAJA
            if (dto.Cena <= 0)
                throw new Exception("Cena mora biti veća od 0.");

            var novoDelo = new UmetnickoDelo
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Slika = dto.Slika,
                Tehnika = dto.Tehnika,
                Stil = dto.Stil,
                Dimenzije = dto.Dimenzije,
                UmetnikId = dto.UmetnikId,

                Cena = dto.Cena,
                NaAukciji = false,

                PocetnaCenaAukcije = null,
                TrenutnaCenaAukcije = null,
                AukcijaPocinje = null,
                AukcijaZavrsava = null,

                Status = UmetnickoDeloStatus.Dostupno,
                DatumPostavljanja = DateTime.UtcNow
            };

            _context.UmetnickaDela.Add(novoDelo);
            await _context.SaveChangesAsync();

            return novoDelo;
        }

        // ============================
        // ADD AUCTION ARTWORK
        // ============================
        public async Task<UmetnickoDelo> AddAuctionArtworkAsync(
    KreirajDeloZaAukcijuDTO dto,
    int umetnikId)
        {
            if (dto.PocetnaCenaAukcije <= 0)
                throw new Exception("Početna cena mora biti veća od 0.");

            if (dto.AukcijaZavrsava <= DateTime.UtcNow)
                throw new Exception("Datum završetka mora biti u budućnosti.");

            var delo = new UmetnickoDelo
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Slika = dto.Slika,
                Tehnika = dto.Tehnika,
                Stil = dto.Stil,
                Dimenzije = dto.Dimenzije,

                NaAukciji = true,
                PocetnaCenaAukcije = (float)dto.PocetnaCenaAukcije,
                TrenutnaCenaAukcije = (float)dto.PocetnaCenaAukcije,

                AukcijaPocinje = DateTime.UtcNow,
                AukcijaZavrsava = dto.AukcijaZavrsava,

                Status = UmetnickoDeloStatus.Dostupno,
                DatumPostavljanja = DateTime.UtcNow,

                UmetnikId = umetnikId
            };

            _context.UmetnickaDela.Add(delo);
            await _context.SaveChangesAsync();

            return delo;
        }


        // ============================
        // UPDATE
        // ============================
        public async Task<bool> UpdateArtworkAsync(AzuriranjeUmetnickogDelaDTO dto)
        {
            var delo = await _context.UmetnickaDela.FindAsync(dto.UmetnickoDeloId);
            if (delo == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Naziv))
                delo.Naziv = dto.Naziv;

            if (!string.IsNullOrWhiteSpace(dto.Opis))
                delo.Opis = dto.Opis;

            if (dto.Cena.HasValue)
            {
                if (delo.NaAukciji)
                    throw new Exception("Nije moguće menjati cenu dela koje je na aukciji.");

                if (dto.Cena.Value <= 0)
                    throw new Exception("Cena mora biti veća od 0.");

                delo.Cena = dto.Cena.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Slika))
                delo.Slika = dto.Slika;

            if (!string.IsNullOrWhiteSpace(dto.Tehnika))
                delo.Tehnika = dto.Tehnika;

            if (!string.IsNullOrWhiteSpace(dto.Stil))
                delo.Stil = dto.Stil;

            if (!string.IsNullOrWhiteSpace(dto.Dimenzije))
                delo.Dimenzije = dto.Dimenzije;

            if (dto.Status.HasValue)
                delo.Status = dto.Status.Value;

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

        public async Task<IEnumerable<UmetnickoDelo>> GetArtworksByArtistAsync(int umetnikId)
        {
            return await _context.UmetnickaDela
                .Where(d => d.UmetnikId == umetnikId)
                .Include(d => d.Umetnik)
                .ToListAsync();
        }

        public async Task<IEnumerable<UmetnickoDelo>> SearchArtworksAsync(string keyword)
        {
            return await _context.UmetnickaDela
                .Where(d => d.Naziv.Contains(keyword) || d.Opis.Contains(keyword))
                .Include(d => d.Umetnik)
                .ToListAsync();
        }

        public async Task<IEnumerable<UmetnickoDelo>> GetArtworksByKorisnikIdAsync(string korisnikId)
        {
            return await _context.UmetnickaDela
                .Where(d => d.Umetnik.KorisnikId == korisnikId)
                .Include(d => d.Umetnik)
                .ToListAsync();
        }

    }
}
