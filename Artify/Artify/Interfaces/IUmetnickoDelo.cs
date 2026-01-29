using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.UmetnickoDeloDTO;


namespace Artify.Interfaces
{
    public interface IUmetnickoDelo
    {
        Task<IEnumerable<UmetnickoDelo>> GetAllArtworksAsync();
        Task<UmetnickoDelo> GetArtworkByIdAsync(int UmetnickoDeloId);
        Task<UmetnickoDelo> AddArtworkAsync(KreirajUmetnickoDeloDTO NovoDelo);
        Task<bool> UpdateArtworkAsync(AzuriranjeUmetnickogDelaDTO IzmenjenoDelo);
        Task<bool> DeleteArtworkAsync(int id);
        Task<IEnumerable<UmetnickoDelo>> GetArtworksByArtistAsync(int UmetnikId);
        Task<IEnumerable<UmetnickoDelo>> SearchArtworksAsync(string keyword);
    }
}

