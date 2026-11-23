using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Prescriptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class PrescriptionsController : BaseApiController
    {
        private readonly IPrescriptionService _service;

        public PrescriptionsController(IPrescriptionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<PrescriptionDto>> Create([FromBody] PrescriptionCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetPatientHistory(int patientId)
        {
            var result = await _service.GetPatientHistoryAsync(patientId);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> GetDoctorHistory(int doctorId)
        {
            var result = await _service.GetDoctorHistoryAsync(doctorId);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<PrescriptionDto>> UpdateStatus(int id, [FromQuery] PrescriptionStatus status)
        {
            var result = await _service.UpdateStatusAsync(id, status);
            return Ok(result);
        }
    }
}

