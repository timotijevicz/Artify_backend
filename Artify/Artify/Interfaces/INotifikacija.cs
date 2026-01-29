using Artify.Models;
using Artify.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artify.DTO_klase.NotifikacijaDTO;

namespace Artify.Interfaces
{
    public interface INotifikacija
    {
        Task<IEnumerable<Notifikacija>> GetAllNotificationsAsync(string KorisnikId); // Dohvati sve notifikacije za korisnika
        Task<Notifikacija> GetNotificationByIdAsync(int NotifikacijaId); // Dohvati notifikaciju po ID-u
        Task<Notifikacija> CreateNotificationAsync(KreirajNotifikacijuDTO novaNotifikacija); // Kreiraj novu notifikaciju
        Task<bool> UpdateNotificationAsync(AzurirajNotifikacijuDTO izmenaNotifikacije); // Ažuriraj notifikaciju
        Task<bool> DeleteNotificationAsync(int NotifikacijaId); // Obriši notifikaciju po ID-u
    }
}
