using TourSite.Core.DTOs.User;
using TourSite.Core.Helper;
using TourSite.Core.Specification.Users;

namespace TourSite.Core.Servicies.Contract
{
    public interface IUserService
    {
        Task<PageinationResponse<UserDto>> GetAllUserAsync(UserSpeciParams userSpecParams);
        Task<UserDto> GetUserByIdAsync(string id);
        Task AssignRolesAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
        Task SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
    }
}
