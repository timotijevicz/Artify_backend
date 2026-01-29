using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.UmetnikDTO;

namespace Artify.Interfaces
{
    public interface IUmetnik
    {
        Task<IEnumerable<Umetnik>> GetAllArtistsAsync(); // Dohvati sve umetnike
        Task<Umetnik> GetArtistByIdAsync(int UmetnikId); // Dohvati umetnika po ID-u
        Task<Umetnik> CreateArtistAsync(KreirajUmetnikaDTO noviUmetnik); // Kreiraj novog umetnika
        Task<bool> UpdateArtistAsync(AzurirajUmetnikaDTO izmenaUmetnika); // Ažuriraj umetnika
        Task<bool> DeleteArtistAsync(int UmetnikId); // Obriši umetnika
        Task<bool> ApproveArtistAsync(int UmetnikId); // Odobri umetnika (admin funkcija)
    }
}