using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase;
using Artify.DTO_klase.KategorijeDTO;

namespace Artify.Interfaces
{
    public interface IKategorija
    {
        Task<IEnumerable<Kategorija>> GetAllKategorijeAsync();
        Task<Kategorija> GetKategorijaByIdAsync(int KategorijaId);
        Task<Kategorija> CreateKategorijaAsync(KreiranjeKategorijeDTO NovaKategorijaDTO);
        Task<Kategorija> UpdateKategorijaAsync(AzuriranjeKategorijeDTO IzmenjenaKategorijaDTO);
        Task<bool> DeleteKategorijaAsync(int KategorijaId);
    }
}
