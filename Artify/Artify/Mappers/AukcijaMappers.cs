using AutoMapper;
using Artify.DTO_klase.AukcijskaPonudaDTO;
using Artify.Models;

namespace Artify.Mappers
{
    public class AukcijaMappers : Profile
    {
        public AukcijaMappers()
        {
            // Entity <-> Response DTO
            CreateMap<AukcijskaPonuda, PonudaDTO>();
            CreateMap<PonudaDTO, AukcijskaPonuda>();
            CreateMap<AukcijskaPonuda, FinalizeAukcijaDTO>();
            CreateMap<FinalizeAukcijaDTO, AukcijskaPonuda>();

            // Request DTO -> Entity (NE treba Entity -> Request DTO)
            CreateMap<DodajPonuduDTO, AukcijskaPonuda>()
                .ForMember(d => d.AukcijskaPonudaId, opt => opt.Ignore())
                .ForMember(d => d.KupacId, opt => opt.Ignore())
                .ForMember(d => d.DatumKreiranja, opt => opt.Ignore())
                .ForMember(d => d.UmetnickoDelo, opt => opt.Ignore());
        }
    }
}
