using TourSite.Core.Utilities;

namespace TourSite.Core.Specification.Users
{
    public class UserSpeciParams
    {
        private string? search;
        public string? Search
        {
            get => search;
            set => search = value?.ToLower();
        }
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;

        public int SafePageIndex => ClinicRules.ClampPageIndex(pageIndex);
        public int SafePageSize => ClinicRules.ClampPageSize(pageSize);
    }
}
