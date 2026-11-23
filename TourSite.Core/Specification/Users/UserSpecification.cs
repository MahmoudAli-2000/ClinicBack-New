using TourSite.Core.Entities;

namespace TourSite.Core.Specification.Users
{
    public class UserSpecification : BaseSpecifications<User>
    {
        public UserSpecification(string id) : base(u => u.Id == id)
        {
        }

        public UserSpecification(UserSpeciParams specParams) : base(u =>
            string.IsNullOrEmpty(specParams.Search) ||
            u.FullName.ToLower().Contains(specParams.Search))
        {
            ApplyPag(specParams.SafePageSize, specParams.SafePageIndex);
            AddOrderBy(u => u.FullName);
        }
    }
}
