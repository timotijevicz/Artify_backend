using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.DTO_klase.NotifikacijaDTO;
using Artify.Repositories;
using System.Security.Claims;


namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifikacijaController : ControllerBase
    {
        private readonly INotifikacija _notifikacijaService;

        public NotifikacijaController(INotifikacija notifikacijaService)
        {
            _notifikacijaService = notifikacijaService;
        }

        /// <summary>
        /// Vraća sve notifikacije za određenog korisnika.
        /// </summary>
        [HttpGet("SveNotifikacije/{KorisnikId}")]
        public async Task<IActionResult> GetAllNotifications(string KorisnikId)
        {
            var notifikacije = await _notifikacijaService.GetAllNotificationsAsync(KorisnikId);
            return Ok(notifikacije);
        }

        /// <summary>
        /// Vraća jednu notifikaciju po ID-u.
        /// </summary>
        [HttpGet("{NotifikacijaId:int}")]
        public async Task<IActionResult> GetNotificationById(int NotifikacijaId)
        {
            var notifikacija = await _notifikacijaService.GetNotificationByIdAsync(NotifikacijaId);
            if (notifikacija == null)
                return NotFound(new { Poruka = "Notifikacija nije pronađena." });

            return Ok(notifikacija);
        }

        /// <summary>
        /// Kreira novu notifikaciju.
        /// </summary>
        [HttpPost("KreirajNotifikaciju")]
        public async Task<IActionResult> CreateNotification([FromBody] KreirajNotifikacijuDTO novaNotifikacija)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notifikacija = await _notifikacijaService.CreateNotificationAsync(novaNotifikacija);
            return CreatedAtAction(nameof(GetNotificationById), new { NotifikacijaId = notifikacija.NotifikacijaId }, notifikacija);
        }

        /// <summary>
        /// Ažurira notifikaciju po ID-u.
        /// </summary>
        [HttpPut("AzurirajNotifikaciju")]
        public async Task<IActionResult> UpdateNotification([FromBody] AzurirajNotifikacijuDTO izmenaNotifikacije)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var uspeh = await _notifikacijaService.UpdateNotificationAsync(izmenaNotifikacije);
            if (!uspeh)
                return NotFound(new { Poruka = "Notifikacija nije pronađena." });

            return NoContent();
        }

        /// <summary>
        /// Briše notifikaciju po ID-u.
        /// </summary>
        [HttpDelete("ObrisiNotifikaciju/{NotifikacijaId}")]
        public async Task<IActionResult> DeleteNotification(int NotifikacijaId)
        {
            var uspeh = await _notifikacijaService.DeleteNotificationAsync(NotifikacijaId);
            if (!uspeh)
                return NotFound(new { Poruka = "Notifikacija nije pronađena." });

            return NoContent();
        }
    }

}
