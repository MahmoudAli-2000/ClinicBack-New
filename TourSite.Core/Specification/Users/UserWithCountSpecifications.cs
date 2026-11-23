using TourSite.Core.Entities;

namespace TourSite.Core.Specification.Users
{
    public class UserWithCountSpecifications : BaseSpecifications<User>
    {
        public UserWithCountSpecifications(UserSpeciParams specParams) : base(
            p => string.IsNullOrEmpty(specParams.Search) || p.FullName.ToLower().Contains(specParams.Search!))
        {
        }
    }
}
