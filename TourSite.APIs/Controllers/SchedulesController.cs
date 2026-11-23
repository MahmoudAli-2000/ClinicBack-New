using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Schedules;
using TourSite.Core.Servicies.Contract;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class SchedulesController : BaseApiController
    {
        private readonly IScheduleService _service;

        public SchedulesController(IScheduleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> Create([FromBody] ScheduleCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ScheduleDto>> Update(int id, [FromBody] ScheduleUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Schedule deleted." });
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IReadOnlyList<ScheduleDto>>> GetByDoctor(int doctorId)
        {
            var result = await _service.GetByDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IReadOnlyList<ScheduleDto>>> GetByClinic(int clinicId)
        {
            var result = await _service.GetByClinicAsync(clinicId);
            return Ok(result);
        }

        [HttpPost("timeoffs")]
        public async Task<ActionResult<TimeOffDto>> CreateTimeOff([FromBody] TimeOffCreateDto dto)
        {
            var result = await _service.CreateTimeOffAsync(dto);
            return Ok(result);
        }

        [HttpDelete("timeoffs/{id}")]
        public async Task<ActionResult> DeleteTimeOff(int id)
        {
            await _service.DeleteTimeOffAsync(id);
            return Ok(new { message = "Time off deleted." });
        }

        [HttpGet("timeoffs/{doctorId}")]
        public async Task<ActionResult<IReadOnlyList<TimeOffDto>>> GetTimeOff(int doctorId)
        {
            var result = await _service.GetTimeOffAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("availability")]
        public async Task<ActionResult<IReadOnlyList<AvailabilitySlotDto>>> GetAvailability([FromQuery] AvailabilityQueryDto query)
        {
            var result = await _service.GetAvailabilityAsync(query);
            return Ok(result);
        }
    }
}

