using AutoMapper;
using TechWorldService.Models;
using TechWorldService.DTO;

namespace TechWorldAPI.Profiles
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
