using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core.Entities;
using TourSite.Core.DTOs.User;

namespace TourSite.Core.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile(IConfiguration configuration)
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.ImageCover, o => o.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"));
        }
    }
}
