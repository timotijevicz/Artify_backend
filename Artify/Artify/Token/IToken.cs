using Artify.Models;

namespace Artify.Token
{
    public interface IToken
    {
        string CreateToken(Korisnik korisnik);
    }
}