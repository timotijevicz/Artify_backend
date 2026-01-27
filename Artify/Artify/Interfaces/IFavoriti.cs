using Artify.Models;

namespace Artify.Interfaces
{
    public interface IFavoriti
    {
        Task<List<Favoriti>> GetAllFavoritesByUserId(string KupacId);
        Task<Favoriti> AddToFavorites(string KupacId, int UmetnickoDeloId);
        Task<bool> RemoveFromFavorites(string KupacId, int UmetnickoDeloId);
    }
}
