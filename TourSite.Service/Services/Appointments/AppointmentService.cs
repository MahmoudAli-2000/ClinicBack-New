using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TourSite.Core;
using TourSite.Core.DTOs.Appointments;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly INotificationService _notificationService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _notificationService = notificationService;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk && !_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only staff members can create appointments.");

            return await _unitOfWork.ExecuteInTransactionAsync(
                async () => await CreateInternalAsync(dto, cancellationToken),
                IsolationLevel.Serializable,
                cancellationToken);
        }

        private async Task<AppointmentDto> CreateInternalAsync(AppointmentCreateDto dto, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.PatientId, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            var doctorClinic = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .Include(dc => dc.Doctor)
                .FirstOrDefaultAsync(dc => dc.Id == dto.DoctorClinicId, cancellationToken);
            if (doctorClinic is null)
                throw new AppException(404, "Doctor-clinic assignment not found.");

            var endUtc = dto.ScheduledEndAtUtc ?? dto.ScheduledStartAtUtc.AddMinutes(doctorClinic.Doctor.ConsultationDurationMinutes);
            if (endUtc <= dto.ScheduledStartAtUtc)
                throw new AppException(400, "End time must be later than start time.");

            await ValidateAvailabilityAsync(doctorClinic.Id, doctorClinic.DoctorId, dto.ScheduledStartAtUtc, endUtc, cancellationToken);

            var now = DateTime.UtcNow;
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorClinicId = dto.DoctorClinicId,
                ScheduledStartAtUtc = dto.ScheduledStartAtUtc,
                ScheduledEndAtUtc = endUtc,
                Status = AppointmentStatus.Scheduled,
                BookingSource = dto.BookingSource,
                Reason = dto.Reason,
                Notes = dto.Notes,
                CreatedByUserId = _currentUser.UserId,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                patient.UserId ?? string.Empty,
                "AppointmentCreated",
                "Appointment Scheduled",
                $"Your appointment is scheduled for {dto.ScheduledStartAtUtc:yyyy-MM-dd HH:mm}.",
                appointment.Id,
                cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            return await MapToDtoAsync(appointment, cancellationToken);
        }

        private async Task ValidateAvailabilityAsync(int doctorClinicId, int doctorId, DateTime startAt, DateTime endAt, CancellationToken ct)
        {
            var date = startAt.Date;
            var dayOfWeek = ClinicRules.ToScheduleDayOfWeek(date);

            var schedule = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.DoctorClinicId == doctorClinicId &&
                    s.DayOfWeek == dayOfWeek && s.IsActive &&
                    ClinicRules.EffectiveWindowCovers(date, s.EffectiveFrom, s.EffectiveTo) &&
                    s.StartTime <= startAt.TimeOfDay && s.EndTime >= endAt.TimeOfDay, ct);

            if (schedule is null)
                throw new AppException(409, "The doctor is not available at the requested time slot.");

            var timeOffConflict = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .AnyAsync(t => t.DoctorId == doctorId &&
                    t.Status == TimeOffStatus.Approved &&
                    ClinicRules.RangesOverlap(t.StartAtUtc, t.EndAtUtc, startAt, endAt), ct);

            if (timeOffConflict)
                throw new AppException(409, "The doctor is on leave during the requested time.");

            var overlap = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .AnyAsync(a => a.DoctorClinicId == doctorClinicId &&
                    a.Id != 0 &&
                    ClinicRules.IsBlockingStatus(a.Status) &&
                    ClinicRules.RangesOverlap(a.ScheduledStartAtUtc, a.ScheduledEndAtUtc, startAt, endAt), ct);

            if (overlap)
                throw new AppException(409, "The selected time slot conflicts with another appointment.");
        }

        public async Task<AppointmentDto> RescheduleAsync(int id, AppointmentRescheduleDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk && !_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only staff members can reschedule appointments.");

            return await _unitOfWork.ExecuteInTransactionAsync(
                async () => await RescheduleInternalAsync(id, dto, cancellationToken),
                IsolationLevel.Serializable,
                cancellationToken);
        }

        private async Task<AppointmentDto> RescheduleInternalAsync(int id, AppointmentRescheduleDto dto, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                    .ThenInclude(d => d.Staff)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.Rescheduled))
                throw new AppException(409, $"Cannot reschedule an appointment in {appointment.Status} status.");

            var doctorClinicId = dto.DoctorClinicId ?? appointment.DoctorClinicId;
            var doctorClinic = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .Include(dc => dc.Doctor)
                .FirstOrDefaultAsync(dc => dc.Id == doctorClinicId, cancellationToken);
            if (doctorClinic is null)
                throw new AppException(404, "Doctor-clinic assignment not found.");

            var endUtc = dto.ScheduledEndAtUtc ?? dto.ScheduledStartAtUtc.AddMinutes(doctorClinic.Doctor.ConsultationDurationMinutes);
            if (endUtc <= dto.ScheduledStartAtUtc)
                throw new AppException(400, "End time must be later than start time.");

            appointment.DoctorClinicId = doctorClinicId;
            await ValidateAvailabilityForRescheduleAsync(doctorClinicId, doctorClinic.DoctorId, appointment.Id, dto.ScheduledStartAtUtc, endUtc, cancellationToken);

            appointment.ScheduledStartAtUtc = dto.ScheduledStartAtUtc;
            appointment.ScheduledEndAtUtc = endUtc;
            appointment.Status = AppointmentStatus.Rescheduled;
            appointment.Notes = dto.Notes;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                appointment.PatientId != 0 ? (await GetPatientUserIdAsync(appointment.PatientId, cancellationToken)) ?? string.Empty : string.Empty,
                "AppointmentRescheduled",
                "Appointment Rescheduled",
                $"Your appointment has been rescheduled to {dto.ScheduledStartAtUtc:yyyy-MM-dd HH:mm}.",
                appointment.Id,
                cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        private async Task ValidateAvailabilityForRescheduleAsync(int doctorClinicId, int doctorId, int excludeAppointmentId, DateTime startAt, DateTime endAt, CancellationToken ct)
        {
            var date = startAt.Date;
            var dayOfWeek = ClinicRules.ToScheduleDayOfWeek(date);

            var schedule = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .AnyAsync(s => s.DoctorClinicId == doctorClinicId && s.DayOfWeek == dayOfWeek && s.IsActive &&
                    ClinicRules.EffectiveWindowCovers(date, s.EffectiveFrom, s.EffectiveTo) &&
                    s.StartTime <= startAt.TimeOfDay && s.EndTime >= endAt.TimeOfDay, ct);

            if (!schedule)
                throw new AppException(409, "The doctor is not available at the requested time slot.");

            var timeOffConflict = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .AnyAsync(t => t.DoctorId == doctorId && t.Status == TimeOffStatus.Approved &&
                    ClinicRules.RangesOverlap(t.StartAtUtc, t.EndAtUtc, startAt, endAt), ct);

            if (timeOffConflict)
                throw new AppException(409, "The doctor is on leave during the requested time.");

            var overlap = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .AnyAsync(a => a.DoctorClinicId == doctorClinicId &&
                    a.Id != excludeAppointmentId &&
                    ClinicRules.IsBlockingStatus(a.Status) &&
                    ClinicRules.RangesOverlap(a.ScheduledStartAtUtc, a.ScheduledEndAtUtc, startAt, endAt), ct);

            if (overlap)
                throw new AppException(409, "The selected time slot conflicts with another appointment.");
        }

        public async Task<AppointmentDto> CancelAsync(int id, AppointmentCancelDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk && !_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only staff members can cancel appointments.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.Cancelled))
                throw new AppException(409, $"Cannot cancel an appointment in {appointment.Status} status.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAtUtc = DateTime.UtcNow;
            appointment.CancellationReason = dto.CancellationReason;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                appointment.Patient.UserId ?? string.Empty,
                "AppointmentCancelled",
                "Appointment Cancelled",
                $"Your appointment on {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} has been cancelled.",
                appointment.Id,
                cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        public async Task<AppointmentDto> ConfirmAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only front-desk staff can confirm appointments.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.Confirmed))
                throw new AppException(409, $"Cannot confirm an appointment in {appointment.Status} status.");

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                appointment.Patient.UserId ?? string.Empty,
                "AppointmentConfirmed",
                "Appointment Confirmed",
                $"Your appointment on {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} has been confirmed.",
                appointment.Id,
                cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        public async Task<AppointmentDto> CheckInAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can check in patients.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.CheckedIn))
                throw new AppException(409, $"Cannot check in an appointment in {appointment.Status} status.");

            appointment.Status = AppointmentStatus.CheckedIn;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        public async Task<AppointmentDto> CompleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can complete appointments.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.Completed))
                throw new AppException(409, $"Cannot complete an appointment in {appointment.Status} status.");

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                appointment.Patient.UserId ?? string.Empty,
                "AppointmentCompleted",
                "Appointment Completed",
                $"Your appointment on {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} has been completed.",
                appointment.Id,
                cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        public async Task<AppointmentDto> MarkNoShowAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only front-desk staff can mark appointments as no-show.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (!ClinicRules.CanTransition(appointment.Status, AppointmentStatus.NoShow))
                throw new AppException(409, $"Cannot mark as no-show an appointment in {appointment.Status} status.");

            appointment.Status = AppointmentStatus.NoShow;
            appointment.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _notificationService.PublishAsync(
                appointment.Patient.UserId ?? string.Empty,
                "AppointmentNoShow",
                "Appointment Missed",
                $"Your appointment on {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} was marked as not attended.",
                appointment.Id,
                cancellationToken);

            return await MapToDtoAsync(appointment, cancellationToken);
        }

        public async Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Staff)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Specialty)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            await VerifyAccessAsync(appointment.PatientId, cancellationToken);
            return await MapToDtoAsync(appointment, cancellationToken);
        }

        private async Task VerifyAccessAsync(int patientId, CancellationToken ct)
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
                    throw new AppException(403, "You do not have access to this appointment.");
            }
        }

        public async Task<PageinationResponse<AppointmentDto>> SearchAsync(AppointmentQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var now = DateTime.UtcNow;

            IQueryable<Appointment> queryable = _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Staff)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Specialty)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Clinic)
                .Include(a => a.Review)
                .Include(a => a.Visit);

            if (query.PatientId.HasValue)
                queryable = queryable.Where(a => a.PatientId == query.PatientId.Value);
            if (query.DoctorId.HasValue)
                queryable = queryable.Where(a => a.DoctorClinic.DoctorId == query.DoctorId.Value);
            if (query.ClinicId.HasValue)
                queryable = queryable.Where(a => a.DoctorClinic.ClinicId == query.ClinicId.Value);
            if (query.Status.HasValue)
                queryable = queryable.Where(a => a.Status == query.Status.Value);
            if (query.FromUtc.HasValue)
                queryable = queryable.Where(a => a.ScheduledStartAtUtc >= query.FromUtc.Value);
            if (query.ToUtc.HasValue)
                queryable = queryable.Where(a => a.ScheduledStartAtUtc <= query.ToUtc.Value);

            if (!string.IsNullOrEmpty(searchLower))
            {
                queryable = queryable.Where(a =>
                    a.Patient.FirstName.ToLower().Contains(searchLower) ||
                    a.Patient.LastName.ToLower().Contains(searchLower) ||
                    a.Reason != null && a.Reason.ToLower().Contains(searchLower));
            }

            if (_currentUser.IsPatient)
            {
                var patientIds = _unitOfWork.Repository<Patient>().Query()
                    .AsNoTracking()
                    .Where(p => p.UserId == _currentUser.UserId)
                    .Select(p => p.Id);
                queryable = queryable.Where(a => patientIds.Contains(a.PatientId));
            }

            var count = await queryable.CountAsync(cancellationToken);
            var items = await queryable.OrderByDescending(a => a.ScheduledStartAtUtc)
                .Skip(query.SafePageIndex - 1)
                .Take(query.SafePageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<AppointmentDto>();
            foreach (var item in items)
                dtos.Add(_mapper.Map<AppointmentDto>(item));

            return new PageinationResponse<AppointmentDto>(query.SafePageIndex, query.SafePageSize, count, dtos);
        }

        private async Task<AppointmentDto> MapToDtoAsync(Appointment appointment, CancellationToken ct)
        {
            var loaded = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Staff)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                        .ThenInclude(d => d.Specialty)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Clinic)
                .Include(a => a.Review)
                .Include(a => a.Visit)
                .FirstOrDefaultAsync(a => a.Id == appointment.Id, ct);

            return _mapper.Map<AppointmentDto>(loaded);
        }

        private async Task<string?> GetPatientUserIdAsync(int patientId, CancellationToken ct)
        {
            return await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .Where(p => p.Id == patientId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync(ct);
        }
    }
}








