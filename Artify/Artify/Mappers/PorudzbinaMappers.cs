using AutoMapper;
using Artify.DTO_klase.PorudzbinaDTO;
using Artify.Models;


namespace Artify.Mappers
{
    public class PorudzbinaMappers : Profile
    {
        public PorudzbinaMappers() 
        {
            CreateMap<KreiranjePorudzbineDTO, Porudzbina>();
            CreateMap<Porudzbina, KreiranjePorudzbineDTO>();

            CreateMap<AzuriranjePorudzbineDTO, Porudzbina>();
            CreateMap<Porudzbina, AzuriranjePorudzbineDTO>();
        }
    }
}
