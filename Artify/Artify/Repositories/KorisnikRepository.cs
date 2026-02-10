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
            return await _userManager.FindByIdAsync(korisnikId);
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

            // Briši umetnička dela (ti već brišeš sva, ids ti nisu potrebni)
            foreach (var delo in umetnik.UmetnickaDela)
                _context.UmetnickaDela.Remove(delo);

            _context.Umetnici.Remove(umetnik);
            await _context.SaveChangesAsync();
        }


        public async Task<string> ChangePasswordAsync(PromenaLozinkeKorisnikaDTO dto)
        {
           
            return "Koristi PromenaLozinkeMojProfil (ChangePasswordMyAsync) - ova ruta je deprecated.";
        }

      

        public async Task<object?> GetMyProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                ImeIPrezime = user.ImeIPrezime,
                Roles = roles
            };
        }

        public async Task<string> ChangePasswordMyAsync(string userId, PromenaLozinkeKorisnikaDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Korisnik nije pronađen.";

            if (dto.NovaLozinka != dto.PotvrdaNoveLozinke)
                return "Nova lozinka i potvrda se ne poklapaju.";

            var result = await _userManager.ChangePasswordAsync(user, dto.TrenutnaLozinka, dto.NovaLozinka);
            if (!result.Succeeded)
            {
                var err = result.Errors.FirstOrDefault()?.Description ?? "Greška pri promeni lozinke.";
                return err;
            }

            return "Lozinka uspešno promenjena.";
        }

        public async Task<string> ChangeEmailMyAsync(string userId, PromenaEmailKorisnikaDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Korisnik nije pronađen.";

            var passOk = await _userManager.CheckPasswordAsync(user, dto.Lozinka);
            if (!passOk) return "Pogrešna lozinka.";

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NoviEmail);
            var emailResult = await _userManager.ChangeEmailAsync(user, dto.NoviEmail, token);

            if (!emailResult.Succeeded)
            {
                var err = emailResult.Errors.FirstOrDefault()?.Description ?? "Greška pri promeni email-a.";
                return err;
            }

            user.UserName = dto.NoviEmail;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var err = updateResult.Errors.FirstOrDefault()?.Description ?? "Greška pri ažuriranju korisnika.";
                return err;
            }

            return "Email uspešno promenjen.";
        }

        public async Task<string> DeleteMyAccountAsync(string userId, BrisanjeNalogaDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Korisnik nije pronađen.";

            var passOk = await _userManager.CheckPasswordAsync(user, dto.Lozinka);
            if (!passOk) return "Pogrešna lozinka.";

            // ✅ TRANSAKCIJA (da ne ostane polu-obrisano)
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Recenzije
                if (_context.Recenzije != null)
                {
                    var recenzije = await _context.Recenzije
                        .Where(r => r.KorisnikId == userId)
                        .ToListAsync();

                    if (recenzije.Any())
                    {
                        _context.Recenzije.RemoveRange(recenzije);
                        await _context.SaveChangesAsync();
                    }
                }

                // 2) Porudzbine (i stavke porudzbine ako postoje)
                if (_context.Porudzbine != null)
                {
                    // Ako imaš stavke porudzbine, obriši prvo njih:
                    // var porudzbineIds = await _context.Porudzbine.Where(p => p.KorisnikId == userId).Select(p => p.Id).ToListAsync();
                    // var stavke = await _context.StavkePorudzbine.Where(s => porudzbineIds.Contains(s.PorudzbinaId)).ToListAsync();
                    // _context.StavkePorudzbine.RemoveRange(stavke);

                    var porudzbine = await _context.Porudzbine
                        .Where(p => p.KorisnikId == userId)
                        .ToListAsync();

                    if (porudzbine.Any())
                    {
                        _context.Porudzbine.RemoveRange(porudzbine);
                        await _context.SaveChangesAsync();
                    }
                }

                // 3) Notifikacije (ako imaš FK na korisnika)
                // Ako je Notifikacija vezana preko KorisnikId, dodaj i ovo:
                // var notif = await _context.Notifikacije.Where(n => n.KorisnikId == userId).ToListAsync();
                // _context.Notifikacije.RemoveRange(notif);

                // 4) Ako je umetnik: obriši umetnik-profil + dela
                var umetnik = await _context.Umetnici
                    .Include(u => u.UmetnickaDela)
                    .FirstOrDefaultAsync(u => u.KorisnikId == userId);

                if (umetnik != null)
                {
                    if (umetnik.UmetnickaDela != null && umetnik.UmetnickaDela.Any())
                    {
                        _context.UmetnickaDela.RemoveRange(umetnik.UmetnickaDela);
                        await _context.SaveChangesAsync();
                    }

                    _context.Umetnici.Remove(umetnik);
                    await _context.SaveChangesAsync();
                }
                // ✅ 5) Jedan SaveChanges pre brisanja user-a (manje upita, sigurnije)
                await _context.SaveChangesAsync();

                // 5) Obriši Identity user-a
                var delRes = await _userManager.DeleteAsync(user);
                if (!delRes.Succeeded)
                {
                    var err = delRes.Errors.FirstOrDefault()?.Description ?? "Greška pri brisanju naloga.";
                    await tx.RollbackAsync();
                    return err;
                }

                await tx.CommitAsync();
                return "Nalog obrisan.";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                // možeš logovati ex.Message
                return "Greška pri brisanju naloga (zavisne tabele).";
            }
        }


    }
}
