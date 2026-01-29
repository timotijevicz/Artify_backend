using AutoMapper;
using Artify.DTO_klase.NotifikacijaDTO;
using Artify.Models;

namespace Artify.Mappers
{
    public class NotifikacijaMappers : Profile
    {
       
        public NotifikacijaMappers()
        {
            CreateMap<KreirajNotifikacijuDTO, Notifikacija>();
            CreateMap<Notifikacija, KreirajNotifikacijuDTO>();

            CreateMap<AzurirajNotifikacijuDTO, Notifikacija>();
            CreateMap<Notifikacija, AzurirajNotifikacijuDTO>();
        }
        
    }
}
