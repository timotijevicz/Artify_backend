using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.KorisnikDTO;
using Artify.DTO_klase.UmetnikDTO;

namespace Artify.Interfaces
{
    public interface IKorisnik
    {
        Task<IEnumerable<Korisnik>> GetAllUsersAsync(); // Dohvati sve korisnike
        Task<Korisnik> GetUserByIdAsync(string KorisnikId); // Dohvati korisnika po ID-u
        Task<string> RegisterAsync(RegistracijaKorisnikaDTO RegistracijaDTO); // Registracija korisnika
        Task<LoginResponseDTO?> LoginAsync(LogovanjeKorisnikaDTO dto); // Prijava korisnika
        Task<string> ChangePasswordAsync(PromenaLozinkeKorisnikaDTO PromenaLozinkeDTO); // Promena lozinke
        Task DeleteUserAsync(string KorisnikId); // Brisanje korisnika
        Task<string> RegisterArtistAsync(RegistracijaUmetnikaDTO dto);
        Task DeleteArtistAsync(string UmetnikId, IEnumerable<int> UmetnickaDelaIds); // Brisanje umetnika i dela
        Task LogoutAsync(); // Odjava korisnika
    }
}
