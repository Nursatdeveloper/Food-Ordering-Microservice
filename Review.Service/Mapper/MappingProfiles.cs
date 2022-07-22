using AutoMapper;
using Reviews.Service.Models;
using static Reviews.Service.Dtos;

namespace Reviews.Service.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateReviewDto, Review>();
        }
    }
}
