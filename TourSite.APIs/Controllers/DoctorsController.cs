using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Doctors;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    public class DoctorsController : BaseApiController
    {
        private readonly IDoctorService _service;

        public DoctorsController(IDoctorService service)
        {
            _service = service;
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPost]
        public async Task<ActionResult<DoctorDetailsDto>> Create([FromForm] DoctorCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDetailsDto>> Update(int id, [FromForm] DoctorUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDetailsDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PageinationResponse<DoctorListDto>>> Search([FromQuery] DoctorQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpPost("clinics")]
        public async Task<ActionResult<DoctorClinicDto>> AssignClinic([FromBody] DoctorClinicAssignDto dto)
        {
            var result = await _service.AssignClinicAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminManager)]
        [HttpDelete("clinics/{id}")]
        public async Task<ActionResult> UnassignClinic(int id)
        {
            await _service.UnassignClinicAsync(id);
            return Ok(new { message = "Doctor unassigned from clinic." });
        }

        [Authorize]
        [HttpGet("{id}/clinics")]
        public async Task<ActionResult<IReadOnlyList<DoctorClinicDto>>> GetClinics(int id)
        {
            var result = await _service.GetDoctorClinicsAsync(id);
            return Ok(result);
        }
    }
}

