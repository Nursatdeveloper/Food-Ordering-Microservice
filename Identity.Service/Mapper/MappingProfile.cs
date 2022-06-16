using AutoMapper;
using Identity.Service.Models;
using static Identity.Service.Dtos;

namespace Identity.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterDto, User>();
        }
    }
}
