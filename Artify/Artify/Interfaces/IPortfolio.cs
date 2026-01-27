using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.PortfolioDTO;

namespace Artify.Interfaces
{
    public interface IPortfolio
    {
        Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync();  // Dohvati sve portfolije
        Task<Portfolio> GetPortfolioByIdAsync(int PortfolioId);  // Dohvati portfolio po ID-u
        Task<IEnumerable<Portfolio>> GetPortfoliosByUserIdAsync(string KorisnikId);  // Dohvati portfolije po ID-u korisnika
        Task<Portfolio> CreatePortfolioAsync(KreiranjePortfoliaDTO dto);  // Kreiranje novog portfolia
        Task<bool> UpdatePortfolioAsync(int PortfolioId, AzuriranjePortfoliaDTO dto);  // Ažuriranje postojećeg portfolia
        Task<bool> DeletePortfolioAsync(int PortfolioId, string UmetnikId); // Parametri za brisanje bez DTO klase


    }
}

