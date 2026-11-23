using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Appointments;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> Create([FromBody] AppointmentCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}/reschedule")]
        public async Task<ActionResult<AppointmentDto>> Reschedule(int id, [FromBody] AppointmentRescheduleDto dto)
        {
            var result = await _service.RescheduleAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<AppointmentDto>> Cancel(int id, [FromBody] AppointmentCancelDto dto)
        {
            var result = await _service.CancelAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/confirm")]
        public async Task<ActionResult<AppointmentDto>> Confirm(int id)
        {
            var result = await _service.ConfirmAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}/checkin")]
        public async Task<ActionResult<AppointmentDto>> CheckIn(int id)
        {
            var result = await _service.CheckInAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<AppointmentDto>> Complete(int id)
        {
            var result = await _service.CompleteAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}/no-show")]
        public async Task<ActionResult<AppointmentDto>> MarkNoShow(int id)
        {
            var result = await _service.MarkNoShowAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PageinationResponse<AppointmentDto>>> Search([FromQuery] AppointmentQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }
    }
}

