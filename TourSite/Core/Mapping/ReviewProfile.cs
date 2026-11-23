using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
           /maps<Review, ReviewDto>();
           /maps<ReviewCreateDto, Review>()
                .ForMember(dest => dest.ReviewedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow));
        }
    }
}