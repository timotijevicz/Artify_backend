using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.PorudzbinaDTO;

namespace Artify.Interfaces
{
    public interface IPorudzbina
    {
        Task<IEnumerable<Porudzbina>> GetAllPorudzbineAsync(); // Dohvati sve porudžbine
        Task<Porudzbina> GetPorudzbinaByIdAsync(int PorudzbinaId); // Dohvati porudžbinu po ID-u
        Task<Porudzbina> CreatePorudzbinaAsync(KreiranjePorudzbineDTO NovaPorudzbinaDTO); // Kreiranje nove porudžbine sa DTO klasom
        Task UpdatePorudzbinaAsync(AzuriranjePorudzbineDTO IzmenaPorudzbineDTO); // Ažuriranje porudžbine sa DTO klasom
        Task<bool> DeletePorudzbinaAsync(int PorudzbinaId); // Brisanje porudžbine po ID-u
        Task<List<Porudzbina>> GetPorudzbineByKorisnikIdAsync(string korisnikId);
        Task<bool> ArchiveAsync(int porudzbinaId, string korisnikId);

        Task<bool> PayAsync(int porudzbinaId, string korisnikId);


    }
}