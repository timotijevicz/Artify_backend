using Artify.Data;
using Artify.Models;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace Artify.GraphQL
{
    public class Query
    {
        private readonly AppDbContext _context;

        public Query(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------
        // UMETNIČKA DELA
        // -------------------------

        [UseFiltering]
        [UseSorting]
        public IQueryable<UmetnickoDelo> GetUmetnickaDela() =>
            _context.UmetnickaDela.AsNoTracking();

        // Delo po ID (koristi tvoj PK: UmetnickoDeloId)
        [UseFiltering]
        [UseSorting]
        public IQueryable<UmetnickoDelo> GetUmetnickoDeloById(int umetnickoDeloId) =>
            _context.UmetnickaDela
                .AsNoTracking()
                .Where(d => d.UmetnickoDeloId == umetnickoDeloId);

        // Dela po umetniku
        [UseFiltering]
        [UseSorting]
        public IQueryable<UmetnickoDelo> GetUmetnickaDelaByUmetnik(int umetnikId) =>
            _context.UmetnickaDela
                .AsNoTracking()
                .Where(d => d.UmetnikId == umetnikId);

        // Samo dostupna dela (primer)
        [UseFiltering]
        [UseSorting]
        public IQueryable<UmetnickoDelo> GetDostupnaDela() =>
            _context.UmetnickaDela
                .AsNoTracking()
                .Where(d => d.Status == UmetnickoDeloStatus.Dostupno);

        // -------------------------
        // UMETNICI
        // -------------------------

        [UseFiltering]
        [UseSorting]
        public IQueryable<Umetnik> GetUmetnici() =>
            _context.Umetnici.AsNoTracking();
    }
}