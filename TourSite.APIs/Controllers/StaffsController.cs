using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Staff;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    public class StaffsController : BaseApiController
    {
        private readonly IStaffService _service;

        public StaffsController(IStaffService service)
        {
            _service = service;
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPost]
        public async Task<ActionResult<StaffDto>> Create([FromForm] StaffCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<StaffDto>> Update(int id, [FromForm] StaffUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpGet]
        public async Task<ActionResult<PageinationResponse<StaffDto>>> Search([FromQuery] StaffQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> SetStatus(int id, [FromQuery] StaffStatus status)
        {
            await _service.SetStatusAsync(id, status);
            return Ok(new { message = "Staff status updated." });
        }
    }
}

