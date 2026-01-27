using AutoMapper;
using Artify.DTO_klase.PortfolioDTO ;
using Artify.Models;


namespace Artify.Mappers
{
    public class PortfolioMappers : Profile
    {
        public PortfolioMappers() 
        {
            CreateMap<KreiranjePortfoliaDTO, Portfolio>();
            CreateMap<Portfolio, KreiranjePortfoliaDTO>();

            CreateMap<AzuriranjePortfoliaDTO, Portfolio>();
            CreateMap<Portfolio, AzuriranjePortfoliaDTO>();
        }
    }
}
