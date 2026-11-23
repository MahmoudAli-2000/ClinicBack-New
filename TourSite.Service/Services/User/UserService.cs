using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Common;
using TourSite.Core.DTOs.User;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.Users;
using TourSite.Core.Utilities;

namespace TourSite.Service.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PageinationResponse<UserDto>> GetAllUserAsync(UserSpeciParams userSpecParams)
        {
            var spec = new UserSpecification(userSpecParams);
            var data = _mapper.Map<IEnumerable<UserDto>>(await _unitOfWork.Repository<User>().GetAllSpecAsync(spec));
            var count = await _unitOfWork.Repository<User>().GetCountAsync(new UserWithCountSpecifications(userSpecParams));
            return new PageinationResponse<UserDto>(userSpecParams.SafePageIndex, userSpecParams.SafePageSize, count, data);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var spec = new UserSpecification(id);
            var user = await _unitOfWork.Repository<User>().GetByIdSpecAsync(spec);
            return user is null ? throw new AppException(404, "User not found.") : _mapper.Map<UserDto>(user);
        }

        public async Task AssignRolesAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AppException(404, "User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (roles.Any())
                await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task SetActiveAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AppException(404, "User not found.");

            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);
        }
    }
}






