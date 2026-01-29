using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Artify.Interfaces
{
    public interface IFavoriti
    {
        Task<List<Favoriti>> GetAllFavoritesByUserId(string KorisnikId);
        Task<Favoriti> AddToFavorites(string KorisnikId, int UmetnickoDeloId);
        Task<bool> RemoveFromFavorites(string KorisnikId, int UmetnickoDeloId);
    }
}
