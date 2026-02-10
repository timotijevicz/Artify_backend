using AutoMapper;
using Artify.DTO_klase.KorisnikDTO  ;
using Artify.Models;

namespace Artify.Mappers
{
    public class KorisnikMappers : Profile
    {
        public KorisnikMappers() 
        {
            CreateMap<LogovanjeKorisnikaDTO, Korisnik>();
            CreateMap<Korisnik, LogovanjeKorisnikaDTO>();

            CreateMap<PromenaLozinkeKorisnikaDTO, Korisnik>();
            CreateMap<Korisnik, PromenaLozinkeKorisnikaDTO>();

            CreateMap<LoginResponseDTO, Korisnik>();
            CreateMap<Korisnik, LoginResponseDTO>();

            CreateMap<BrisanjeNalogaDTO, Korisnik>();
            CreateMap<Korisnik, BrisanjeNalogaDTO>();

            CreateMap<PromenaEmailKorisnikaDTO, Korisnik>();
            CreateMap<Korisnik, PromenaEmailKorisnikaDTO>();

            CreateMap<UserDTO, Korisnik>();
            CreateMap<Korisnik, UserDTO>();
        }
    }
}
