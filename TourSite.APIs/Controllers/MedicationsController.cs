using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Medications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class MedicationsController : BaseApiController
    {
        private readonly IMedicationService _service;

        public MedicationsController(IMedicationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<MedicationDto>> Create([FromForm] MedicationCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicationDto>> Update(int id, [FromForm] MedicationUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicationDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PageinationResponse<MedicationDto>>> Search([FromQuery] PagedQueryDto query)
        {
            var result = await _service.SearchAsync(query);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> SetActive(int id, [FromQuery] bool isActive)
        {
            await _service.SetActiveAsync(id, isActive);
            return Ok(new { message = "Medication status updated." });
        }
    }
}

