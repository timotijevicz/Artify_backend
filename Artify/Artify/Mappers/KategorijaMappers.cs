using AutoMapper;
using Artify.DTO_klase.KategorijeDTO;
using Artify.Models;

namespace Artify.Mappers
{
    public class KategorijaMappers : Profile
    {
        public KategorijaMappers() 
        {
            CreateMap<AzuriranjeKategorijeDTO, Kategorija>();
            CreateMap<Kategorija, AzuriranjeKategorijeDTO>();

            CreateMap<KreiranjeKategorijeDTO, Kategorija>();
            CreateMap<Kategorija, KreiranjeKategorijeDTO>();


        }
    }
}
