using Artify.Interfaces;
using Artify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artify.Data;
using Microsoft.EntityFrameworkCore;
using Artify.DTO_klase.KorisnikDTO;
using Microsoft.AspNetCore.Identity;

namespace Artify.Repositories

{
    public class KorisnikRepository : IKorisnik
    {
        private readonly AppDbContext _context;

        
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;

        public KorisnikRepository(UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IEnumerable<Korisnik>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Korisnik> GetUserByIdAsync(string KorisnikId)
        {
            return await _userManager.FindByIdAsync(KorisnikId);
        }

        public async Task<string> RegisterAsync(RegistracijaKorisnikaDTO RegistracijaDTO)
        {
            var korisnik = new Korisnik
            {
                ImeIPrezime= RegistracijaDTO.ImeIPrezime,
                Email = RegistracijaDTO.Email,
                UserName = RegistracijaDTO.Email,
                DatumRegistracije = DateTime.Now
            };

            var result = await _userManager.CreateAsync(korisnik, RegistracijaDTO.Lozinka);

            if (result.Succeeded)
            {
                return "Registracija uspešna.";
            }

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<string> LoginAsync(LogovanjeKorisnikaDTO LogovanjeDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(
                LogovanjeDTO.Email,
                LogovanjeDTO.Lozinka,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return "Prijava uspešna.";
            }

            return "Neispravno korisničko ime ili lozinka.";
        }

        public async Task<string> ChangePasswordAsync(PromenaLozinkeKorisnikaDTO PromenaLozinkeDTO)
        {
            var korisnik = await _userManager.FindByIdAsync(PromenaLozinkeDTO.KorisnikId);

            if (korisnik == null)
            {
                return "Korisnik nije pronađen.";
            }

            var result = await _userManager.ChangePasswordAsync(
                korisnik,
                PromenaLozinkeDTO.TrenutnaLozinka,
                PromenaLozinkeDTO.NovaLozinka
            );

            if (result.Succeeded)
            {
                return "Lozinka uspešno promenjena.";
            }

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        // Brisanje korisnika
        public async Task DeleteUserAsync(string KorisnikId)
        {
            var korisnik = await _context.Users.FindAsync(KorisnikId);
            if (korisnik == null)
            {
                throw new Exception("Korisnik nije pronađen.");
            }

            _context.Users.Remove(korisnik);
            await _context.SaveChangesAsync();
        }

        // Brisanje umetnika
        public async Task DeleteArtistAsync(string UmetnikId, IEnumerable<int> UmetnickaDelaIds)
        {
            var umetnik = await _context.Users.Include(u => u.UmetnickaDela).FirstOrDefaultAsync(u => u.Id == UmetnikId);
            if (umetnik == null)
            {
                throw new Exception("Umetnik nije pronađen.");
            }

            foreach (var deloId in UmetnickaDelaIds)
            {
                var umetnickoDelo = await _context.UmetnickaDela.FindAsync(deloId);
                if (umetnickoDelo != null)
                {
                    _context.UmetnickaDela.Remove(umetnickoDelo);
                }
            }

            _context.Users.Remove(umetnik);
            await _context.SaveChangesAsync();
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}


