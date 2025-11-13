using AutoMapper;
using GadgetHubAPI.DTO;
using GadgetHubAPI.Models;

namespace GadgetHubAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer mappings
            

            // Product mappings
            CreateMap<ProductRequestDTO, Product>();
            CreateMap<Product, ProductResponseDTO>()
                // Price is not stored in Product entity; it's provided by quotation service
                .ForMember(dest => dest.Price, opt => opt.Ignore());
        }
    }
}
