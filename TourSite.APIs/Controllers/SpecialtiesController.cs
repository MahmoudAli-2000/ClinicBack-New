using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Specialties;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    public class SpecialtiesController : BaseApiController
    {
        private readonly ISpecialtyService _service;

        public SpecialtiesController(ISpecialtyService service)
        {
            _service = service;
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPost]
        public async Task<ActionResult<SpecialtyDto>> Create([FromForm] SpecialtyCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<SpecialtyDto>> Update(int id, [FromForm] SpecialtyUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Specialty deleted." });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialtyDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PageinationResponse<SpecialtyDto>>> Search([FromQuery] PagedQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }
    }
}

