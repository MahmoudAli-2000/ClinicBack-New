using TourSite.Core.Utilities;

namespace TourSite.Core.DTOs.Common
{
    public class PagedQueryDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }

        public int SafePageIndex => ClinicRules.ClampPageIndex(PageIndex);
        public int SafePageSize => ClinicRules.ClampPageSize(PageSize);
        public string? SearchLower => string.IsNullOrWhiteSpace(Search) ? null : Search.Trim().ToLower();
    }
}
