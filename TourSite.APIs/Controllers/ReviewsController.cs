using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Reviews;
using TourSite.Core.Servicies.Contract;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> Create([FromBody] ReviewCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetDoctorReviews(int doctorId)
        {
            var result = await _service.GetDoctorReviewsAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetClinicReviews(int clinicId)
        {
            var result = await _service.GetClinicReviewsAsync(clinicId);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}/stats")]
        public async Task<ActionResult<ReviewStatsDto>> GetDoctorStats(int doctorId)
        {
            var result = await _service.GetDoctorStatsAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("clinic/{clinicId}/stats")]
        public async Task<ActionResult<ReviewStatsDto>> GetClinicStats(int clinicId)
        {
            var result = await _service.GetClinicStatsAsync(clinicId);
            return Ok(result);
        }
    }
}

