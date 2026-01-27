using AutoMapper;
using Artify.DTO_klase.UmetnickoDeloDTO;
using Artify.Models;

namespace Artify.Mappers
{
    public class UmetnickoDeloMappers : Profile
    {
       public UmetnickoDeloMappers() 
       {
            CreateMap<KreirajUmetnickoDeloDTO, UmetnickoDelo>();
            CreateMap<UmetnickoDelo, KreirajUmetnickoDeloDTO>();

            CreateMap<AzuriranjeUmetnickogDelaDTO, UmetnickoDelo>();
            CreateMap<UmetnickoDelo, AzuriranjeUmetnickogDelaDTO>();
        }
    }
}
