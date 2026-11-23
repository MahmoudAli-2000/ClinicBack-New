
using AutoMapper;
using TourSite.Core.DTOs.Reviews;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
           CreateMap<Review, ReviewDto>();
           CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.ReviewedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}