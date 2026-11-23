using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs;
using TourSite.Core.DTOs.User;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.Users;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    [Authorize(Roles = AppRoles.AdminManager)]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<PageinationResponse<UserDto>>> GetAll([FromQuery] UserSpeciParams specParams)
        {
            var result = await _userService.GetAllUserAsync(specParams);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPut("{id}/roles")]
        public async Task<ActionResult> AssignRoles(string id, [FromBody] AssignRolesDto dto)
        {
            await _userService.AssignRolesAsync(id, dto.Roles);
            return Ok(new { message = "Roles updated." });
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> SetActive(string id, [FromQuery] bool isActive)
        {
            await _userService.SetActiveAsync(id, isActive);
            return Ok(new { message = "User status updated." });
        }
    }
}

