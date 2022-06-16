using AutoMapper;
using Catalog.Service.Models;

namespace Catalog.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PublishRestaurantDto, Restaurant>();
            CreateMap<PublishRestaurantAddressDto, Address>();
            CreateMap<PublishFoodCategoryDto, FoodCategory>();
            CreateMap<PublishFoodDto, Food>();
        }
    }
}