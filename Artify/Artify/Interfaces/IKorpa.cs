using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Artify.Interfaces
{
    public interface IKorpa
    {
        Task<IEnumerable<Korpa>> GetAllCartsAsync();
        Task<Korpa> GetCartByIdAsync(int cartId);
        Task<Korpa> AddCartAsync(Korpa newCart);
        Task UpdateCartAsync(Korpa updatedCart);
        Task DeleteCartAsync(int cartId);
    }
}
