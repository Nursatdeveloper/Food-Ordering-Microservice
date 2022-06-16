using AutoMapper;
using Order.Service.Models;
using static Order.Service.Dtos;

namespace Order.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderPublishedDto, FoodOrder>();
        }
    }
}
