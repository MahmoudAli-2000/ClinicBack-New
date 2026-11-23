using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Clinics;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    public class ClinicsController : BaseApiController
    {
        private readonly IClinicService _service;

        public ClinicsController(IClinicService service)
        {
            _service = service;
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPost]
        public async Task<ActionResult<ClinicDto>> Create([FromForm] ClinicCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ClinicDto>> Update(int id, [FromForm] ClinicUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PageinationResponse<ClinicDto>>> Search([FromQuery] PagedQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            await _service.SetActiveAsync(id, isActive);
            return Ok(new { message = "Clinic status updated." });
        }
    }
}

