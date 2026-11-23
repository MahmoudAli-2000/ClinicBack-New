using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.Service.Services.Auth
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool IsAdmin => IsInRole(AppRoles.Admin);

        public bool IsDoctor => IsInRole(AppRoles.Doctor);

        public bool IsPatient => IsInRole(AppRoles.Patient);

        public bool IsFrontDesk => IsInRole(AppRoles.Admin) || IsInRole(AppRoles.Manager) || IsInRole(AppRoles.Receptionist);

        public bool IsClinicalStaff => IsInRole(AppRoles.Admin) || IsInRole(AppRoles.Manager) || IsInRole(AppRoles.Doctor) || IsInRole(AppRoles.Nurse);

        public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}





