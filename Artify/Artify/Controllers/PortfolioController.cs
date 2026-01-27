using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;
using Artify.DTO_klase.PortfolioDTO;


namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolio _portfolioService;

        public PortfolioController(IPortfolio portfolioService)
        {
            _portfolioService = portfolioService;
        }

        /// <summary>
        /// Vraća listu svih portfolija.
        /// </summary>
        /// <returns>HTTP 200 sa listom portfolija.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPortfolios()
        {
            var portfoliji = await _portfolioService.GetAllPortfoliosAsync();
            return Ok(portfoliji);
        }

        /// <summary>
        /// Vraća portfolio na osnovu njegovog ID-a.
        /// </summary>
        /// <param name="id">ID portfolija koji se traži.</param>
        /// <returns>HTTP 200 sa portfolijom ili HTTP 404 ako nije pronađen.</returns>
        [HttpGet("{PortfolioId:int}")]
        public async Task<IActionResult> GetPortfolioById(int PortfolioId)
        {
            var portfolio = await _portfolioService.GetPortfolioByIdAsync(PortfolioId);

            if (portfolio == null)
            {
                return NotFound(new { Poruka = $"Portfolio sa ID-jem {PortfolioId} nije pronađen." });
            }

            return Ok(portfolio);
        }

        /// <summary>
        /// Vraća listu portfolija umetnika na osnovu ID-ja korisnika.
        /// </summary>
        /// <param name="korisnikId">ID korisnika čiji portfoliji se traže.</param>
        /// <returns>HTTP 200 sa listom portfolija.</returns>
        [HttpGet("user/{korisnikId}")]
        public async Task<IActionResult> GetPortfoliosByUserId(string korisnikId)
        {
            var portfoliji = await _portfolioService.GetPortfoliosByUserIdAsync(korisnikId);
            return Ok(portfoliji);
        }

        /// <summary>
        /// Kreira novi portfolio.
        /// </summary>
        /// <param name="dto">DTO objekat sa podacima za kreiranje portfolija.</param>
        /// <returns>HTTP 201 sa podacima o novom portfoliju ili HTTP 400 u slučaju greške.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePortfolio([FromBody] KreiranjePortfoliaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var portfolio = await _portfolioService.CreatePortfolioAsync(dto);
            return CreatedAtAction(nameof(GetPortfolioById), new { id = portfolio.PortfolioId }, portfolio);
        }

        /// <summary>
        /// Ažurira postojeći portfolio.
        /// </summary>
        /// <param name="id">ID portfolija koji se ažurira.</param>
        /// <param name="dto">DTO objekat sa podacima za ažuriranje portfolija.</param>
        /// <returns>HTTP 204 ako je ažuriranje uspešno, HTTP 404 ili 400 u slučaju greške.</returns>
        [HttpPut("{PortfolioId:int}")]
        public async Task<IActionResult> UpdatePortfolio(int PortfolioId, [FromBody] AzuriranjePortfoliaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rezultat = await _portfolioService.UpdatePortfolioAsync(PortfolioId, dto);

            if (!rezultat)
            {
                return NotFound(new { Poruka = $"Portfolio sa ID-jem {PortfolioId} nije pronađen ili nije moguće ažurirati." });
            }

            return NoContent();
        }

        /// <summary>
        /// Briše portfolio na osnovu ID-a.
        /// </summary>
        /// <param name="id">ID portfolija koji se briše.</param>
        /// <param name="umetnikId">ID umetnika kome pripada portfolio.</param>
        /// <returns>HTTP 204 ako je uspešno obrisan, HTTP 404 u slučaju greške.</returns>
        [HttpDelete("{PortfolioId:int}")]
        public async Task<IActionResult> DeletePortfolio(int PortfolioId, [FromQuery] string umetnikId)
        {
            var rezultat = await _portfolioService.DeletePortfolioAsync(PortfolioId, umetnikId);

            if (!rezultat)
            {
                return NotFound(new { Poruka = $"Portfolio sa ID-jem {PortfolioId} nije pronađen ili ne pripada umetniku sa ID-jem {umetnikId}." });
            }

            return NoContent();
        }
    }
}
