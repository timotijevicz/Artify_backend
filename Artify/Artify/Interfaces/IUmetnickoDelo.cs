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
        Task<UmetnickoDelo> GetArtworkByIdAsync(int umetnickoDeloId);

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

    }
}

