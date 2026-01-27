using Artify.Interfaces;
using Artify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Artify.Repositories;
using System.Security.Claims;


namespace Artify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KorpaController : ControllerBase
    {
        private readonly IKorpa _korpaService;

        public KorpaController(IKorpa korpaService)
        {
            _korpaService = korpaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var korpe = await _korpaService.GetAllCartsAsync();
            return Ok(korpe);
        }

        [HttpGet("{KorpaId:int}")]
        public async Task<IActionResult> GetCartById(int KorpaId)
        {
            try
            {
                var korpa = await _korpaService.GetCartByIdAsync(KorpaId);
                return Ok(korpa);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Poruka = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] Korpa novaKorpa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var kreiranaKorpa = await _korpaService.AddCartAsync(novaKorpa);
                return CreatedAtAction(nameof(GetCartById), new { id = kreiranaKorpa.KorpaId }, kreiranaKorpa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Poruka = ex.Message });
            }
        }

        [HttpPut("{KorpaId:int}")]
        public async Task<IActionResult> UpdateCart(int KorpaId, [FromBody] Korpa azuriranaKorpa)
        {
            if (KorpaId != azuriranaKorpa.KorpaId)
            {
                return BadRequest("ID korpe u zahtevu se ne poklapa sa ID-jem u telu zahteva.");
            }

            try
            {
                await _korpaService.UpdateCartAsync(azuriranaKorpa);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Poruka = ex.Message });
            }
        }

        [HttpDelete("{KorpaId:int}")]
        public async Task<IActionResult> DeleteCart(int KorpaId)
        {
            try
            {
                await _korpaService.DeleteCartAsync(KorpaId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Poruka = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Poruka = ex.Message });
            }
        }
    }
}
