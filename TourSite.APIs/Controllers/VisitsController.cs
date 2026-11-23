using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Visits;
using TourSite.Core.Servicies.Contract;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class VisitsController : BaseApiController
    {
        private readonly IVisitService _service;

        public VisitsController(IVisitService service)
        {
            _service = service;
        }

        [HttpPost("from-appointment/{appointmentId}")]
        public async Task<ActionResult<VisitDto>> StartFromAppointment(int appointmentId, [FromBody] VisitStartDto dto)
        {
            var result = await _service.StartFromAppointmentAsync(appointmentId, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<VisitDto>> Complete(int id, [FromBody] VisitUpdateDto dto)
        {
            var result = await _service.CompleteAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/notes")]
        public async Task<ActionResult<VisitDto>> UpdateNotes(int id, [FromBody] VisitUpdateDto dto)
        {
            var result = await _service.UpdateNotesAsync(id, dto);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IReadOnlyList<VisitDto>>> GetPatientHistory(int patientId)
        {
            var result = await _service.GetPatientHistoryAsync(patientId);
            return Ok(result);
        }

        [HttpPost("{visitId}/vitals")]
        public async Task<ActionResult<VitalSignDto>> AddVital(int visitId, [FromBody] VitalSignCreateDto dto)
        {
            var result = await _service.AddVitalAsync(visitId, dto);
            return Ok(result);
        }

        [HttpPut("vitals/{vitalId}")]
        public async Task<ActionResult<VitalSignDto>> UpdateVital(int vitalId, [FromBody] VitalSignCreateDto dto)
        {
            var result = await _service.UpdateVitalAsync(vitalId, dto);
            return Ok(result);
        }

        [HttpDelete("vitals/{vitalId}")]
        public async Task<ActionResult> DeleteVital(int vitalId)
        {
            await _service.DeleteVitalAsync(vitalId);
            return Ok(new { message = "Vital sign deleted." });
        }

        [HttpPost("{visitId}/diagnoses")]
        public async Task<ActionResult<DiagnosisDto>> AddDiagnosis(int visitId, [FromBody] DiagnosisCreateDto dto)
        {
            var result = await _service.AddDiagnosisAsync(visitId, dto);
            return Ok(result);
        }

        [HttpPut("diagnoses/{diagnosisId}")]
        public async Task<ActionResult<DiagnosisDto>> UpdateDiagnosis(int diagnosisId, [FromBody] DiagnosisCreateDto dto)
        {
            var result = await _service.UpdateDiagnosisAsync(diagnosisId, dto);
            return Ok(result);
        }

        [HttpDelete("diagnoses/{diagnosisId}")]
        public async Task<ActionResult> DeleteDiagnosis(int diagnosisId)
        {
            await _service.DeleteDiagnosisAsync(diagnosisId);
            return Ok(new { message = "Diagnosis deleted." });
        }

        [HttpGet("patient/{patientId}/diagnoses")]
        public async Task<ActionResult<IReadOnlyList<DiagnosisDto>>> GetPatientDiagnoses(int patientId)
        {
            var result = await _service.GetPatientDiagnosesAsync(patientId);
            return Ok(result);
        }
    }
}

