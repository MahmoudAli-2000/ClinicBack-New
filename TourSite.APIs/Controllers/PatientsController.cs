using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Patients;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    public class PatientsController : BaseApiController
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [Authorize(Roles = AppRoles.FrontDesk)]
        [HttpPost]
        public async Task<ActionResult<PatientDetailsDto>> Create([FromForm] PatientCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.FrontDesk)]
        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDetailsDto>> Update(int id, [FromForm] PatientUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDetailsDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.FrontDesk)]
        [HttpGet]
        public async Task<ActionResult<PageinationResponse<PatientListDto>>> Search([FromQuery] PatientQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [Authorize(Roles = AppRoles.FrontDesk)]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> SetStatus(int id, [FromQuery] PatientStatus status)
        {
            await _service.SetStatusAsync(id, status);
            return Ok(new { message = "Patient status updated." });
        }
    }
}

