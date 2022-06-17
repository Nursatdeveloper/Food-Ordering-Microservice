using AutoMapper;
using Catalog.Service.Models;
using Catalog.Service.PublishItems;

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
            CreateMap<CreateOrderDto, PublishOrder>();

        }
    }
}