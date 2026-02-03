using AutoMapper;
using Artify.DTO_klase.UmetnikDTO;
using Artify.Models;

namespace Artify.Mappers
{
    public class UmetnikMappers : Profile
    {
        public UmetnikMappers()
        {
            // Mapiranje za update (AzurirajUmetnikaDTO <-> Umetnik)
            CreateMap<AzurirajUmetnikaDTO, Umetnik>();
            CreateMap<Umetnik, AzurirajUmetnikaDTO>();

            // Mapiranje za kreiranje (KreirajUmetnikaDTO -> Umetnik)
            CreateMap<KreirajUmetnikaDTO, Umetnik>();
            CreateMap<Umetnik, KreirajUmetnikaDTO>();

            CreateMap<RegistracijaUmetnikaDTO, Umetnik>();
            CreateMap<Umetnik, RegistracijaUmetnikaDTO>();

        }
    }
}
