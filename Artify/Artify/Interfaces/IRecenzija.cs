using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.RecenzijaDTO;

namespace Artify.Interfaces
{
    public interface IRecenzija
    {
        Task<IEnumerable<Recenzija>> GetSveRecenzijeAsync(); 
        Task<Recenzija> GetRecenzijaByIdAsync(int RecenzijaId); // Dohvati recenziju po ID-u
        Task<IEnumerable<Recenzija>> GetRecenzijeZaUmetnickoDeloAsync(int UmetnickoDeloId); // Dohvati sve recenzije za određeno umetničko delo
        Task<Recenzija> KreirajRecenzijuAsync(KreirajRecenzijuDTO NovaRecenzijaDTO); // Kreiranje recenzije
        Task<Recenzija> AzurirajRecenzijuAsync(AzurirajRecenzijuDTO IzmenjenaRecenzijaDTO); // Ažuriranje recenzije
        Task<bool> ObrisiRecenzijuAsync(int RecenzijaId); // Brisanje recenzije po ID-u
    }
}
