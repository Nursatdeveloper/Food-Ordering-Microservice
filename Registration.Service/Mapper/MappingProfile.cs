using AutoMapper;
using Registration.Service.Models;
using Registration.Service.PublishItems;

namespace Registration.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRestaurantDto, Restaurant>();
            CreateMap<CreateAddressDto, Address>();
            CreateMap<CreateFoodDto, Food>();
            CreateMap<Food, FoodViewDto>();
            CreateMap<CreateFoodCategoryDto, FoodCategory>();
            CreateMap<FoodCategory, FoodCategoryViewDto>();
            CreateMap<Address, PublishRestaurantAddress>();
            CreateMap<Food, PublishFood>();
            CreateMap<FoodCategory, PublishFoodCategory>();
            CreateMap<CreateOrderStreamingConnection, OrderStreamingConnection>();
            CreateMap<OrderStreamingConnection, PublishOrderStreamingConnection>();
        }
    }
}