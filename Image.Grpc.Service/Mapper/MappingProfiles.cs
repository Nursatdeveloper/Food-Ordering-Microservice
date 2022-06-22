using AutoMapper;
using Image.Grpc.Service.Models;

namespace Image.Grpc.Service.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<PostFoodImageRequest, FoodImage>();
            CreateMap<PostRestaurantImageRequest, RestaurantImage>();
        }
    }
}
