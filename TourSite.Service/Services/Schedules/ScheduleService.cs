using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Schedules;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Schedules
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<ScheduleDto> CreateAsync(ScheduleCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can create schedules.");

            if (dto.EndTime <= dto.StartTime)
                throw new AppException(400, "End time must be later than start time.");

            var doctorClinic = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(dc => dc.Id == dto.DoctorClinicId, cancellationToken);
            if (doctorClinic is null)
                throw new AppException(404, "Doctor-clinic assignment not found.");

            var overlap = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .AnyAsync(s => s.DoctorClinicId == dto.DoctorClinicId &&
                               s.DayOfWeek == dto.DayOfWeek &&
                               s.IsActive &&
                               ClinicRules.TimeRangesOverlap(s.StartTime, s.EndTime, dto.StartTime, dto.EndTime),
                    cancellationToken);

            if (overlap)
                throw new AppException(409, "Schedule overlaps with an existing schedule for the same day.");

            var schedule = _mapper.Map<DoctorSchedule>(dto);
            schedule.CreatedAtUtc = DateTime.UtcNow;
            schedule.IsActive = true;

            await _unitOfWork.Repository<DoctorSchedule>().AddAsync(schedule, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> UpdateAsync(int id, ScheduleUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can update schedules.");

            if (dto.EndTime <= dto.StartTime)
                throw new AppException(400, "End time must be later than start time.");

            var schedule = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (schedule is null)
                throw new AppException(404, "Schedule not found.");

            var overlap = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .AnyAsync(s => s.Id != id &&
                               s.DoctorClinicId == schedule.DoctorClinicId &&
                               s.DayOfWeek == dto.DayOfWeek &&
                               s.IsActive &&
                               ClinicRules.TimeRangesOverlap(s.StartTime, s.EndTime, dto.StartTime, dto.EndTime),
                    cancellationToken);

            if (overlap)
                throw new AppException(409, "Schedule overlaps with an existing schedule for the same day.");

            schedule.DayOfWeek = dto.DayOfWeek;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.SlotDurationMinutes = dto.SlotDurationMinutes;
            schedule.EffectiveFrom = dto.EffectiveFrom;
            schedule.EffectiveTo = dto.EffectiveTo;
            schedule.IsActive = dto.IsActive;

            _unitOfWork.Repository<DoctorSchedule>().Update(schedule);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can delete schedules.");

            var schedule = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (schedule is null)
                throw new AppException(404, "Schedule not found.");

            _unitOfWork.Repository<DoctorSchedule>().Delete(schedule);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ScheduleDto>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == doctorId, cancellationToken);
            if (!exists)
                throw new AppException(404, "Doctor not found.");

            var schedules = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .Include(s => s.DoctorClinic)
                .ThenInclude(dc => dc.Clinic)
                .Where(s => s.DoctorClinic.DoctorId == doctorId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<ScheduleDto>>(schedules);
        }

        public async Task<IReadOnlyList<ScheduleDto>> GetByClinicAsync(int clinicId, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .AnyAsync(c => c.Id == clinicId, cancellationToken);
            if (!exists)
                throw new AppException(404, "Clinic not found.");

            var schedules = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .Include(s => s.DoctorClinic)
                .ThenInclude(dc => dc.Doctor)
                .ThenInclude(d => d.Staff)
                .Where(s => s.DoctorClinic.ClinicId == clinicId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<ScheduleDto>>(schedules);
        }

        public async Task<TimeOffDto> CreateTimeOffAsync(TimeOffCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can create time-off entries.");

            if (dto.EndAtUtc <= dto.StartAtUtc)
                throw new AppException(400, "End time must be later than start time.");

            var doctorExists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == dto.DoctorId, cancellationToken);
            if (!doctorExists)
                throw new AppException(404, "Doctor not found.");

            var overlap = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .AnyAsync(t => t.DoctorId == dto.DoctorId &&
                               t.Status == TimeOffStatus.Approved &&
                               ClinicRules.RangesOverlap(t.StartAtUtc, t.EndAtUtc, dto.StartAtUtc, dto.EndAtUtc),
                    cancellationToken);

            if (overlap)
                throw new AppException(409, "Time-off period overlaps with an existing approved time-off.");

            var timeOff = _mapper.Map<DoctorTimeOff>(dto);
            timeOff.Status = TimeOffStatus.Approved;
            timeOff.CreatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<DoctorTimeOff>().AddAsync(timeOff, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<TimeOffDto>(timeOff);
        }

        public async Task DeleteTimeOffAsync(int id, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can delete time-off entries.");

            var timeOff = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (timeOff is null)
                throw new AppException(404, "Time-off not found.");

            _unitOfWork.Repository<DoctorTimeOff>().Delete(timeOff);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TimeOffDto>> GetTimeOffAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == doctorId, cancellationToken);
            if (!exists)
                throw new AppException(404, "Doctor not found.");

            var timeOffs = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .Where(t => t.DoctorId == doctorId)
                .OrderBy(t => t.StartAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TimeOffDto>>(timeOffs);
        }

        public async Task<IReadOnlyList<AvailabilitySlotDto>> GetAvailabilityAsync(AvailabilityQueryDto query, CancellationToken cancellationToken = default)
        {
            var doctorClinics = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .Include(dc => dc.Clinic)
                .Include(dc => dc.Doctor)
                .ThenInclude(d => d.Staff)
                .Where(dc => dc.DoctorId == query.DoctorId && dc.IsActive)
                .ToListAsync(cancellationToken);

            if (!doctorClinics.Any())
                throw new AppException(404, "Doctor is not assigned to any active clinic.");

            if (query.ClinicId.HasValue)
                doctorClinics = doctorClinics.Where(dc => dc.ClinicId == query.ClinicId.Value).ToList();

            var date = query.DateUtc.Date;
            var dayOfWeek = ClinicRules.ToScheduleDayOfWeek(date);

            var activeSchedules = await _unitOfWork.Repository<DoctorSchedule>().Query()
                .AsNoTracking()
                .Include(s => s.DoctorClinic)
                .ThenInclude(dc => dc.Clinic)
                .Where(s => s.IsActive &&
                           ClinicRules.EffectiveWindowCovers(date, s.EffectiveFrom, s.EffectiveTo))
                .ToListAsync(cancellationToken);

            var filteredSchedules = activeSchedules.Where(s =>
                doctorClinics.Select(dc => dc.Id).Contains(s.DoctorClinicId) &&
                s.DayOfWeek == dayOfWeek).ToList();

            var timeOffs = await _unitOfWork.Repository<DoctorTimeOff>().Query()
                .AsNoTracking()
                .Include(t => t.Clinic)
                .Where(t => t.DoctorId == query.DoctorId &&
                           t.Status == TimeOffStatus.Approved &&
                           t.StartAtUtc <= date.AddDays(1) &&
                           t.EndAtUtc >= date)
                .ToListAsync(cancellationToken);

            var bookedAppointments = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Where(a => ClinicRules.IsBlockingStatus(a.Status) &&
                           filteredSchedules.Select(s => s.DoctorClinicId).Contains(a.DoctorClinicId) &&
                           a.ScheduledStartAtUtc >= date &&
                           a.ScheduledEndAtUtc <= date.AddDays(1))
                .ToListAsync(cancellationToken);

            var result = new List<AvailabilitySlotDto>();

            foreach (var schedule in filteredSchedules)
            {
                var slots = ClinicRules.BuildSlots(schedule.StartTime, schedule.EndTime, schedule.SlotDurationMinutes);

                foreach (var (slotStart, slotEnd) in slots)
                {
                    var slotStartUtc = date.Add(slotStart);
                    var slotEndUtc = date.Add(slotEnd);

                    if (timeOffs.Any(t => ClinicRules.RangesOverlap(slotStartUtc, slotEndUtc, t.StartAtUtc, t.EndAtUtc)))
                        continue;

                    if (bookedAppointments.Any(a => ClinicRules.RangesOverlap(slotStartUtc, slotEndUtc, a.ScheduledStartAtUtc, a.ScheduledEndAtUtc)))
                        continue;

                    result.Add(new AvailabilitySlotDto
                    {
                        DoctorClinicId = schedule.DoctorClinicId,
                        ClinicId = schedule.DoctorClinic.ClinicId,
                        ClinicName = schedule.DoctorClinic.Clinic.ClinicName,
                        StartAtUtc = slotStartUtc,
                        EndAtUtc = slotEndUtc
                    });
                }
            }

            result = result.OrderBy(s => s.StartAtUtc).ToList();
            return result;
        }
    }
}





