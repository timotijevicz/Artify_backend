using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Artify.Models;
using Microsoft.AspNetCore.Identity;
using Artify.Token;


namespace Artify.Token
{
    public class TokenService : IToken
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Korisnik> _userManager;

        public TokenService(IConfiguration config, UserManager<Korisnik> userManager)
        {
            _config = config;
            _userManager = userManager;

        }

        public string CreateToken(Korisnik korisnik)
        {
            // Pretpostavlja se da imate pristup UserManager da biste dohvatili uloge korisnika.
            var roles = _userManager.GetRolesAsync(korisnik).Result;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, korisnik.Id),
                new Claim(ClaimTypes.Name, korisnik.UserName),
                new Claim(ClaimTypes.Email, korisnik.Email)
            };

            // Dodavanje uloga u token kao claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // ClaimTypes.Role je ključ za uloge
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));  // Key iz appsettings
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
