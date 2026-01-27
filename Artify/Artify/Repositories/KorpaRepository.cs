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
    public class KorpaRepository : IKorpa
    {
        private readonly AppDbContext _context;

        public KorpaRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Korpa>> GetAllCartsAsync()
        {
            return await _context.Set<Korpa>().Include(k => k.Kupac).Include(k => k.Porudzbine).ToListAsync();
        }

        public async Task<Korpa> GetCartByIdAsync(int cartId)
        {
            var cart = await _context.Set<Korpa>()
                .Include(k => k.Kupac)
                .Include(k => k.Porudzbine)
                .FirstOrDefaultAsync(k => k.KorpaId == cartId);

            if (cart == null)
            {
                throw new KeyNotFoundException($"Cart with ID {cartId} was not found.");
            }

            return cart;
        }

        public async Task<Korpa> AddCartAsync(Korpa newCart)
        {
            if (newCart == null)
            {
                throw new ArgumentNullException(nameof(newCart));
            }

            await _context.Set<Korpa>().AddAsync(newCart);
            await _context.SaveChangesAsync();
            return newCart;
        }

        public async Task UpdateCartAsync(Korpa updatedCart)
        {
            if (updatedCart == null)
            {
                throw new ArgumentNullException(nameof(updatedCart));
            }

            _context.Set<Korpa>().Update(updatedCart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartAsync(int cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            _context.Set<Korpa>().Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}

