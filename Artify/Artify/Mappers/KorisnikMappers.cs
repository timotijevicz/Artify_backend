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

            CreateMap<RegistracijaKorisnikaDTO, Korisnik>();
            CreateMap<Korisnik, RegistracijaKorisnikaDTO>();
        }
    }
}
