using AutoMapper;
using Artify.DTO_klase.RecenzijaDTO;
using Artify.Models;


namespace Artify.Mappers
{
    public class RecenzijaMappers : Profile
    {
        public RecenzijaMappers() 
        {
            CreateMap<KreirajRecenzijuDTO, Recenzija>();
            CreateMap<Recenzija, KreirajRecenzijuDTO>();

            CreateMap<AzurirajRecenzijuDTO, Recenzija>();
            CreateMap<Recenzija, AzurirajRecenzijuDTO>();
        }
    }
}
