using Artify.Models;
using Artify.DTO_klase.UmetnickoDeloDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Artify.Interfaces
{
    public interface IUmetnickoDelo
    {
        Task<IEnumerable<UmetnickoDelo>> GetAllArtworksAsync();

        // može biti null (repo koristi FirstOrDefaultAsync)
        Task<UmetnickoDelo?> GetArtworkByIdAsync(int umetnickoDeloId);

        Task<UmetnickoDelo> AddArtworkAsync(KreirajUmetnickoDeloDTO dto);

        Task<UmetnickoDelo> AddAuctionArtworkAsync(
            KreirajDeloZaAukcijuDTO dto,
            int umetnikId
        );

        Task<bool> UpdateArtworkAsync(AzuriranjeUmetnickogDelaDTO dto);
        Task<bool> DeleteArtworkAsync(int id);

        Task<IEnumerable<UmetnickoDelo>> GetArtworksByArtistAsync(int umetnikId);
        Task<IEnumerable<UmetnickoDelo>> GetArtworksByKorisnikIdAsync(string korisnikId);

        Task<IEnumerable<UmetnickoDelo>> SearchArtworksAsync(string keyword);

        // rešava DodajNovoDelo i aukciju kad umetnik nema nijedno delo
        Task<int?> GetUmetnikIdByKorisnikIdAsync(string korisnikId);

        Task<bool> DeactivateArtworkAsync(int umetnickoDeloId, string korisnikId);
    }
}
