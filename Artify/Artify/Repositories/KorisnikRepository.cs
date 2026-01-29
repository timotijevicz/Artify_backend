using Artify.Interfaces;
using Artify.Models;
using Artify.Data;
using Artify.DTO_klase.KorisnikDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artify.Repositories
{
    public class KorisnikRepository : IKorisnik
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;

        public KorisnikRepository(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IEnumerable<Korisnik>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Korisnik> GetUserByIdAsync(int korisnikId)
        {
            // UserManager koristi string ID, pa pretvaramo int u string
            return await _userManager.FindByIdAsync(korisnikId.ToString());
        }

        public async Task<string> RegisterAsync(RegistracijaKorisnikaDTO dto)
        {
            var korisnik = new Korisnik
            {
                ImeIPrezime = dto.ImeIPrezime,
                Email = dto.Email ?? "",
                UserName = dto.Email ?? "",
                DatumRegistracije = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(korisnik, dto.Lozinka ?? "");

            if (result.Succeeded)
                return "Registracija uspešna.";

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<string> LoginAsync(LogovanjeKorisnikaDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                dto.Email ?? "",
                dto.Lozinka ?? "",
                isPersistent: false,
                lockoutOnFailure: false
            );

            return result.Succeeded
                ? "Prijava uspešna."
                : "Neispravno korisničko ime ili lozinka.";
        }

        public async Task<string> ChangePasswordAsync(PromenaLozinkeKorisnikaDTO dto)
        {
            var korisnik = await _userManager.FindByIdAsync(dto.KorisnikId.ToString());

            if (korisnik == null)
                return "Korisnik nije pronađen.";

            var result = await _userManager.ChangePasswordAsync(
                korisnik,
                dto.TrenutnaLozinka ?? "",
                dto.NovaLozinka ?? ""
            );

            return result.Succeeded
                ? "Lozinka uspešno promenjena."
                : string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task DeleteUserAsync(int korisnikId)
        {
            var korisnik = await _context.Users.FindAsync(korisnikId);
            if (korisnik == null)
                throw new Exception("Korisnik nije pronađen.");

            _context.Users.Remove(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteArtistAsync(string korisnikId, IEnumerable<int> umetnickaDelaIds)
        {
            var umetnik = await _context.Umetnici
                .Include(u => u.UmetnickaDela)
                .FirstOrDefaultAsync(u => u.KorisnikId == korisnikId);

            if (umetnik == null)
                throw new Exception("Umetnik nije pronađen.");

            // Briši umetnička dela
            foreach (var delo in umetnik.UmetnickaDela)
            {
                _context.UmetnickaDela.Remove(delo);
            }

            // Briši umetnika
            _context.Umetnici.Remove(umetnik);

            await _context.SaveChangesAsync();
        }
    }
}
