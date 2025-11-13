using AutoMapper;
using GadgetCentralService.Models;
using GadgetCentralService.DTO;

namespace GadgetCentralService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductWriteDTO, Product>();
            CreateMap<Product, ProductReadDTO>();
        }
    }
}
