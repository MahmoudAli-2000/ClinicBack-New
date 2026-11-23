using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Reviews;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.Service.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<ReviewDto> CreateAsync(ReviewCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsPatient)
                throw new AppException(403, "Only patients can submit reviews.");

            if (!dto.DoctorRating.HasValue && !dto.ClinicRating.HasValue && !dto.StaffRating.HasValue)
                throw new AppException(400, "At least one rating is required.");

            if (dto.DoctorRating.HasValue && !ClinicRules.IsValidRating(dto.DoctorRating) ||
                dto.ClinicRating.HasValue && !ClinicRules.IsValidRating(dto.ClinicRating) ||
                dto.StaffRating.HasValue && !ClinicRules.IsValidRating(dto.StaffRating))
                throw new AppException(400, "Ratings must be between 1 and 5.");

            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken);
            if (patient is null)
                throw new AppException(403, "You must be a registered patient to submit reviews.");

            var appointment = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId, cancellationToken);
            if (appointment is null)
                throw new AppException(404, "Appointment not found.");

            if (appointment.PatientId != patient.Id)
                throw new AppException(403, "You can only review your own appointments.");

            if (!ClinicRules.IsReviewEligible(appointment.Status))
                throw new AppException(409, "You can only review completed appointments.");

            var existing = await _unitOfWork.Repository<Review>().Query()
                .AsNoTracking()
                .AnyAsync(r => r.AppointmentId == dto.AppointmentId, cancellationToken);
            if (existing)
                throw new AppException(409, "A review already exists for this appointment.");

            var review = _mapper.Map<Review>(dto);
            review.ReviewedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Review>().AddAsync(review, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<IReadOnlyList<ReviewDto>> GetDoctorReviewsAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var doctorExists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == doctorId, cancellationToken);
            if (!doctorExists)
                throw new AppException(404, "Doctor not found.");

            var reviews = await _unitOfWork.Repository<Review>().Query()
                .AsNoTracking()
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.DoctorClinic)
                        .ThenInclude(dc => dc.Doctor)
                .Where(r => r.Appointment.DoctorClinic.DoctorId == doctorId)
                .OrderByDescending(r => r.ReviewedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
        }

        public async Task<IReadOnlyList<ReviewDto>> GetClinicReviewsAsync(int clinicId, CancellationToken cancellationToken = default)
        {
            var clinicExists = await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .AnyAsync(c => c.Id == clinicId, cancellationToken);
            if (!clinicExists)
                throw new AppException(404, "Clinic not found.");

            var reviews = await _unitOfWork.Repository<Review>().Query()
                .AsNoTracking()
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.DoctorClinic)
                        .ThenInclude(dc => dc.Clinic)
                .Where(r => r.Appointment.DoctorClinic.ClinicId == clinicId)
                .OrderByDescending(r => r.ReviewedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews);
        }

        public async Task<ReviewStatsDto> GetDoctorStatsAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var reviews = await GetDoctorReviewsAsync(doctorId, cancellationToken);
            return BuildStats(reviews);
        }

        public async Task<ReviewStatsDto> GetClinicStatsAsync(int clinicId, CancellationToken cancellationToken = default)
        {
            var reviews = await GetClinicReviewsAsync(clinicId, cancellationToken);
            return BuildStats(reviews);
        }

        private static ReviewStatsDto BuildStats(IReadOnlyList<ReviewDto> reviews)
        {
            return new ReviewStatsDto
            {
                Count = reviews.Count,
                AverageDoctorRating = reviews.Where(r => r.DoctorRating.HasValue).Select(r => (double)r.DoctorRating!.Value).DefaultIfEmpty(0).Average(),
                AverageClinicRating = reviews.Where(r => r.ClinicRating.HasValue).Select(r => (double)r.ClinicRating!.Value).DefaultIfEmpty(0).Average(),
                AverageStaffRating = reviews.Where(r => r.StaffRating.HasValue).Select(r => (double)r.StaffRating!.Value).DefaultIfEmpty(0).Average()
            };
        }
    }
}



