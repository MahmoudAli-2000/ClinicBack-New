using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Visits;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Visits
{
    public class VisitService : IVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;

        public VisitService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<VisitDto> StartFromAppointmentAsync(int appointmentId, VisitStartDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can start visits.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (appointment.Status != AppointmentStatus.CheckedIn &&
                appointment.Status != AppointmentStatus.InProgress)
                throw new AppException(409, $"Cannot start a visit for an appointment in {appointment.Status} status.");

            if (await _unitOfWork.Repository<Visit>().Query().AnyAsync(v => v.AppointmentId == appointmentId, cancellationToken))
                throw new AppException(409, "A visit already exists for this appointment.");

            var doctorId = appointment.DoctorClinic.DoctorId;
            if (!_currentUser.IsAdmin && _currentUser.IsDoctor)
            {
                var staff = await _unitOfWork.Repository<Staff>().Query()
                    .AsNoTracking()
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);
                if (staff?.Doctor?.Id != doctorId)
                    throw new AppException(403, "You can only start visits for your own appointments.");
            }

            appointment.Status = AppointmentStatus.InProgress;
            _unitOfWork.Repository<Appointment>().Update(appointment);

            var visit = new Visit
            {
                AppointmentId = appointmentId,
                CheckInAtUtc = DateTime.UtcNow,
                ChiefComplaint = dto.ChiefComplaint,
                Status = VisitStatus.InProgress,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Visit>().AddAsync(visit, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await GetByIdAsync(visit.Id, cancellationToken);
        }

        public async Task<VisitDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var visit = await _unitOfWork.Repository<Visit>().Query()
                .AsNoTracking()
                .Include(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(v => v.Appointment)
                    .ThenInclude(a => a.DoctorClinic)
                        .ThenInclude(dc => dc.Doctor)
                            .ThenInclude(d => d.Staff)
                .Include(v => v.VitalSigns)
                .Include(v => v.Diagnoses)
                .Include(v => v.Prescriptions)
                    .ThenInclude(p => p.Items)
                        .ThenInclude(pi => pi.Medication)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

            if (visit is null)
                throw new AppException(404, "Visit not found.");

            await VerifyPatientAccessAsync(visit.Appointment.PatientId, cancellationToken);

            return _mapper.Map<VisitDto>(visit);
        }

        private async Task VerifyPatientAccessAsync(int patientId, CancellationToken ct)
        {
            if (_currentUser.IsAdmin || _currentUser.IsFrontDesk || _currentUser.IsClinicalStaff)
                return;

            if (_currentUser.IsPatient)
            {
                var patientUserId = await _unitOfWork.Repository<Patient>().Query()
                    .AsNoTracking()
                    .Where(p => p.Id == patientId)
                    .Select(p => p.UserId)
                    .FirstOrDefaultAsync(ct);

                if (patientUserId != _currentUser.UserId)
                    throw new AppException(403, "You do not have access to this visit.");
            }
        }

        public async Task<VisitDto> CompleteAsync(int id, VisitUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can complete visits.");

            var visit = await _unitOfWork.Repository<Visit>().Query()
                .Include(v => v.Appointment)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
            if (visit is null)
                throw new AppException(404, "Visit not found.");

            if (visit.Status == VisitStatus.Completed)
                throw new AppException(409, "Visit is already completed.");

            if (dto.Status.HasValue)
                visit.Status = dto.Status.Value;
            else
                visit.Status = VisitStatus.Completed;

            visit.VisitEndAtUtc = DateTime.UtcNow;
            visit.UpdatedAtUtc = DateTime.UtcNow;

            if (dto.DoctorNotes is not null)
                visit.DoctorNotes = dto.DoctorNotes;

            _unitOfWork.Repository<Visit>().Update(visit);

            if (visit.Appointment != null && visit.Appointment.Status == AppointmentStatus.InProgress)
            {
                visit.Appointment.Status = AppointmentStatus.Completed;
                _unitOfWork.Repository<Appointment>().Update(visit.Appointment);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<VisitDto> UpdateNotesAsync(int id, VisitUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can update visit notes.");

            var visit = await _unitOfWork.Repository<Visit>().Query()
                .Include(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
            if (visit is null)
                throw new AppException(404, "Visit not found.");

            await VerifyPatientAccessAsync(visit.Appointment.PatientId, cancellationToken);

            if (dto.ChiefComplaint is not null)
                visit.ChiefComplaint = dto.ChiefComplaint;
            if (dto.DoctorNotes is not null)
                visit.DoctorNotes = dto.DoctorNotes;
            if (dto.Status.HasValue)
                visit.Status = dto.Status.Value;

            visit.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Visit>().Update(visit);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<VisitDto>> GetPatientHistoryAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            if (_currentUser.IsPatient)
            {
                if (patient.UserId != _currentUser.UserId)
                    throw new AppException(403, "You can only view your own visit history.");
            }
            else if (!_currentUser.IsAuthenticated)
            {
                throw new AppException(403, "Authentication required.");
            }

            var visits = await _unitOfWork.Repository<Visit>().Query()
                .AsNoTracking()
                .Include(v => v.Appointment)
                    .ThenInclude(a => a.DoctorClinic)
                        .ThenInclude(dc => dc.Doctor)
                            .ThenInclude(d => d.Staff)
                .Where(v => v.Appointment.PatientId == patientId)
                .OrderByDescending(v => v.VisitStartAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<VisitDto>>(visits);
        }

        public async Task<VitalSignDto> AddVitalAsync(int visitId, VitalSignCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can record vital signs.");

            if (!ClinicRules.IsValidVital(dto.WeightKg, dto.HeightCm, dto.SystolicBP, dto.DiastolicBP,
                dto.HeartRateBpm, dto.TemperatureC, dto.OxygenSaturationPct, dto.RespiratoryRate))
                throw new AppException(400, "One or more vital sign values are outside acceptable ranges.");

            var visit = await _unitOfWork.Repository<Visit>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);
            if (visit is null)
                throw new AppException(404, "Visit not found.");

            var staff = await _unitOfWork.Repository<Staff>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);

            var vital = _mapper.Map<VitalSign>(dto);
            vital.VisitId = visitId;
            vital.RecordedByStaffId = staff?.Id;
            vital.RecordedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<VitalSign>().AddAsync(vital, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<VitalSignDto>(vital);
        }

        public async Task<VitalSignDto> UpdateVitalAsync(int vitalId, VitalSignCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can update vital signs.");

            var vital = await _unitOfWork.Repository<VitalSign>().Query()
                .FirstOrDefaultAsync(v => v.Id == vitalId, cancellationToken);
            if (vital is null)
                throw new AppException(404, "Vital sign not found.");

            vital.WeightKg = dto.WeightKg;
            vital.HeightCm = dto.HeightCm;
            vital.SystolicBP = dto.SystolicBP;
            vital.DiastolicBP = dto.DiastolicBP;
            vital.HeartRateBpm = dto.HeartRateBpm;
            vital.TemperatureC = dto.TemperatureC;
            vital.OxygenSaturationPct = dto.OxygenSaturationPct;
            vital.RespiratoryRate = dto.RespiratoryRate;
            vital.Notes = dto.Notes;

            _unitOfWork.Repository<VitalSign>().Update(vital);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<VitalSignDto>(vital);
        }

        public async Task DeleteVitalAsync(int vitalId, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can delete vital signs.");

            var vital = await _unitOfWork.Repository<VitalSign>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == vitalId, cancellationToken);
            if (vital is null)
                throw new AppException(404, "Vital sign not found.");

            _unitOfWork.Repository<VitalSign>().Delete(vital);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<DiagnosisDto> AddDiagnosisAsync(int visitId, DiagnosisCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can add diagnoses.");

            var visit = await _unitOfWork.Repository<Visit>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == visitId, cancellationToken);
            if (visit is null)
                throw new AppException(404, "Visit not found.");

            int? doctorId = null;
            if (_currentUser.IsDoctor)
            {
                doctorId = await _unitOfWork.Repository<Doctor>().Query()
                    .AsNoTracking()
                    .Where(d => d.Staff.UserId == _currentUser.UserId)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (doctorId.HasValue)
                {
                    var appointmentDoctorId = await _unitOfWork.Repository<Appointment>().Query()
                        .AsNoTracking()
                        .Where(a => a.Id == visit.AppointmentId)
                        .Select(a => a.DoctorClinic.DoctorId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (appointmentDoctorId != doctorId)
                        throw new AppException(403, "You can only add diagnoses to your own patients.");
                }
            }

            var diagnosis = _mapper.Map<Diagnosis>(dto);
            diagnosis.VisitId = visitId;
            diagnosis.DiagnosedByDoctorId = doctorId;
            diagnosis.DiagnosedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Diagnosis>().AddAsync(diagnosis, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<DiagnosisDto>(diagnosis);
        }

        public async Task<DiagnosisDto> UpdateDiagnosisAsync(int diagnosisId, DiagnosisCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can update diagnoses.");

            var diagnosis = await _unitOfWork.Repository<Diagnosis>().Query()
                .Include(d => d.Visit)
                    .ThenInclude(v => v.Appointment)
                .FirstOrDefaultAsync(d => d.Id == diagnosisId, cancellationToken);
            if (diagnosis is null)
                throw new AppException(404, "Diagnosis not found.");

            await VerifyDiagnosisAccessAsync(diagnosis, cancellationToken);

            diagnosis.DiagnosisCode = dto.DiagnosisCode;
            diagnosis.DiagnosisName = dto.DiagnosisName;
            diagnosis.DiagnosisType = dto.DiagnosisType;
            diagnosis.Notes = dto.Notes;

            _unitOfWork.Repository<Diagnosis>().Update(diagnosis);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<DiagnosisDto>(diagnosis);
        }

        private async Task VerifyDiagnosisAccessAsync(Diagnosis diagnosis, CancellationToken ct)
        {
            if (_currentUser.IsAdmin || _currentUser.IsInRole(AppRoles.Nurse))
                return;

            if (_currentUser.IsDoctor)
            {
                var doctorId = await _unitOfWork.Repository<Doctor>().Query()
                    .AsNoTracking()
                    .Where(d => d.Staff.UserId == _currentUser.UserId)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(ct);

                var appointmentDoctorId = await _unitOfWork.Repository<Appointment>().Query()
                    .AsNoTracking()
                    .Where(a => a.Id == diagnosis.Visit.AppointmentId)
                    .Select(a => a.DoctorClinic.DoctorId)
                    .FirstOrDefaultAsync(ct);

                if (appointmentDoctorId != doctorId)
                    throw new AppException(403, "You can only update your own diagnoses.");
            }
        }

        public async Task DeleteDiagnosisAsync(int diagnosisId, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can delete diagnoses.");

            var diagnosis = await _unitOfWork.Repository<Diagnosis>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == diagnosisId, cancellationToken);
            if (diagnosis is null)
                throw new AppException(404, "Diagnosis not found.");

            _unitOfWork.Repository<Diagnosis>().Delete(diagnosis);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            if (_currentUser.IsPatient && patient.UserId != _currentUser.UserId)
                throw new AppException(403, "You can only view your own diagnoses.");

            if (!_currentUser.IsAuthenticated)
                throw new AppException(403, "Authentication required.");

            var diagnoses = await _unitOfWork.Repository<Diagnosis>().Query()
                .AsNoTracking()
                .Include(d => d.Visit)
                    .ThenInclude(v => v.Appointment)
                .Where(d => d.Visit.Appointment.PatientId == patientId)
                .OrderByDescending(d => d.DiagnosedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<DiagnosisDto>>(diagnoses);
        }
    }
}





