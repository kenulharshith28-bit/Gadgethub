using AutoMapper;
using ElectroComService.Models;
using ElectroComService.DTO;

namespace ElectroComService.Profiles
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
