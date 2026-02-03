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
using Artify.Token;
using Artify.DTO_klase.UmetnikDTO;

namespace Artify.Repositories
{
    public class KorisnikRepository : IKorisnik
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly IToken _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public KorisnikRepository(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IToken tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<Korisnik>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Korisnik> GetUserByIdAsync(string korisnikId)
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

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            // ✅ DODELA ROLE
            var roleName = "Kupac";

            // (opciono ali preporučeno) proveri da rola postoji
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            var roleResult = await _userManager.AddToRoleAsync(korisnik, roleName);

            if (!roleResult.Succeeded)
                return string.Join(", ", roleResult.Errors.Select(e => e.Description));

            return "Registracija uspešna.";
        }

        public async Task<string> RegisterArtistAsync(RegistracijaUmetnikaDTO dto)
        {
            var roleName = "Umetnik";

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            var korisnik = new Korisnik
            {
                ImeIPrezime = dto.ImeIPrezime,
                Email = dto.Email ?? "",
                UserName = dto.Email ?? "",
                DatumRegistracije = DateTime.UtcNow
            };

            var createRes = await _userManager.CreateAsync(korisnik, dto.Lozinka ?? "");
            if (!createRes.Succeeded)
                return string.Join(", ", createRes.Errors.Select(e => e.Description));

            var roleRes = await _userManager.AddToRoleAsync(korisnik, roleName);
            if (!roleRes.Succeeded)
                return string.Join(", ", roleRes.Errors.Select(e => e.Description));

            // kreiraj umetnika
            var umetnik = new Umetnik
            {
                KorisnikId = korisnik.Id,
                Biografija = dto.Biografija,
                Tehnika = dto.Tehnika,
                Stil = dto.Stil,
                Specijalizacija = dto.Specijalizacija,
                Grad = dto.Grad,
                SlikaUrl = dto.SlikaUrl,
                IsApproved = false,
                IsAvailable = false
            };

            _context.Umetnici.Add(umetnik);
            await _context.SaveChangesAsync();

            return "Registracija umetnika uspešna.";
        }

        public async Task<LoginResponseDTO?> LoginAsync(LogovanjeKorisnikaDTO dto)

        {
            var email = dto.Email ?? "";
            var lozinka = dto.Lozinka ?? "";

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var valid = await _userManager.CheckPasswordAsync(user, lozinka);
            if (!valid) return null;

            var token = _tokenService.CreateToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles
                }
            };
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

        public async Task DeleteUserAsync(string korisnikId)
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
