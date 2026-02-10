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

        [HttpGet("SveNotifikacije/{KorisnikId}")]
        public async Task<IActionResult> GetAllNotifications(string KorisnikId)
        {
            var notifikacije = await _notifikacijaService.GetAllNotificationsAsync(KorisnikId);
            return Ok(notifikacije);
        }

        [HttpGet("{NotifikacijaId:int}")]
        public async Task<IActionResult> GetNotificationById(int NotifikacijaId)
        {
            var notifikacija = await _notifikacijaService.GetNotificationByIdAsync(NotifikacijaId);
            if (notifikacija == null)
                return NotFound(new { Poruka = "Notifikacija nije pronađena." });

            return Ok(notifikacija);
        }

        [HttpPost("KreirajNotifikaciju")]
        public async Task<IActionResult> CreateNotification([FromBody] KreirajNotifikacijuDTO novaNotifikacija)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notifikacija = await _notifikacijaService.CreateNotificationAsync(novaNotifikacija);
            return CreatedAtAction(nameof(GetNotificationById), new { NotifikacijaId = notifikacija.NotifikacijaId }, notifikacija);
        }

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
