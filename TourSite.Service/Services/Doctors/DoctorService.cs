using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Doctors;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Specification;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Doctors
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly IPublicCodeGenerator _codeGenerator;
        private readonly IWebHostEnvironment _env;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser,
            IPublicCodeGenerator codeGenerator, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _codeGenerator = codeGenerator;
            _env = env;
        }

        public async Task<DoctorDetailsDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default)
        {
            var staff = await _unitOfWork.Repository<Staff>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.StaffId, cancellationToken);
            if (staff is null)
                throw new AppException(404, "Staff not found.");

            var specialty = await _unitOfWork.Repository<Specialty>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.SpecialtyId, cancellationToken);
            if (specialty is null)
                throw new AppException(404, "Specialty not found.");

            if (await _unitOfWork.Repository<Doctor>().Query().AnyAsync(d => d.StaffId == dto.StaffId, cancellationToken))
                throw new AppException(409, "Doctor profile already exists for this staff member.");

            if (await _unitOfWork.Repository<Doctor>().Query().AnyAsync(d => d.LicenseNo == dto.LicenseNo, cancellationToken))
                throw new AppException(409, "A doctor with this license number already exists.");

            var doctor = _mapper.Map<Doctor>(dto);
            // 🖼️ حفظ الصورة
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/doctors");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                doctor.ImageCover = $"images/doctors/{fileName}";
            }
            doctor.CreatedAtUtc = DateTime.UtcNow;
            doctor.UpdatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Doctor>().AddAsync(doctor, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            doctor.PublicCode = _codeGenerator.Generate(EntityCodeType.Doctor, doctor.Id, doctor.CreatedAtUtc.Year);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await GetByIdAsync(doctor.Id, cancellationToken);
        }

        public async Task<DoctorDetailsDto> UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().Query()
                .Include(d => d.Staff)
                .Include(d => d.Specialty)
                .Include(d => d.DoctorClinics)
                    .ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (doctor is null)
                throw new AppException(404, "Doctor not found.");

            var specialty = await _unitOfWork.Repository<Specialty>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == dto.SpecialtyId, cancellationToken);
            if (specialty is null)
                throw new AppException(404, "Specialty not found.");

            if (!IsValidLicense(dto.LicenseNo, doctor.LicenseNo) &&
                await _unitOfWork.Repository<Doctor>().Query().AnyAsync(d => d.Id != id && d.LicenseNo == dto.LicenseNo, cancellationToken))
                throw new AppException(409, "A doctor with this license number already exists.");

            doctor.SpecialtyId = dto.SpecialtyId;
            doctor.LicenseNo = dto.LicenseNo;
            doctor.ConsultationDurationMinutes = dto.ConsultationDurationMinutes;
            doctor.DefaultConsultationFee = dto.DefaultConsultationFee;
            doctor.Bio = dto.Bio;
            doctor.IsActive = dto.IsActive;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/doctors");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                doctor.ImageCover = $"images/doctors/{fileName}";
            }
            else if (dto.ImageCover is not null)
                doctor.ImageCover = dto.ImageCover;
            doctor.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Doctor>().Update(doctor);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _mapper.Map<DoctorDetailsDto>(doctor);
            return _mapper.Map<DoctorDetailsDto>(doctor);
        }

        private static bool IsValidLicense(string newLicense, string? existingLicense) =>
            existingLicense == null || newLicense != existingLicense;

        public async Task<DoctorDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .Include(d => d.Staff)
                .Include(d => d.Specialty)
                .Include(d => d.DoctorClinics)
                    .ThenInclude(dc => dc.Clinic)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (doctor is null)
                throw new AppException(404, "Doctor not found.");

            return _mapper.Map<DoctorDetailsDto>(doctor);
        }

        public async Task<PageinationResponse<DoctorListDto>> SearchAsync(DoctorQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Doctor>(d =>
                (string.IsNullOrEmpty(searchLower) ||
                 EF.Functions.Like(d.Staff.FirstName + " " + d.Staff.LastName, $"%{query.Search}%") ||
                 d.LicenseNo.Contains(searchLower)) &&
                (!query.SpecialtyId.HasValue || d.SpecialtyId == query.SpecialtyId.Value) &&
                (!query.IsActive.HasValue || d.IsActive == query.IsActive.Value) &&
                (!query.ClinicId.HasValue || d.DoctorClinics.Any(dc => dc.ClinicId == query.ClinicId.Value)));

            spec.AddInclude(d => d.Staff);
            spec.AddInclude(d => d.Specialty);
            spec.AddOrderBy(d => d.Staff.LastName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Doctor>().GetAllSpecAsync(spec);
            var count = await _unitOfWork.Repository<Doctor>().GetCountAsync(spec);

            return new PageinationResponse<DoctorListDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<DoctorListDto>>(data));
        }

        public async Task<DoctorClinicDto> AssignClinicAsync(DoctorClinicAssignDto dto, CancellationToken cancellationToken = default)
        {
            var doctorExists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == dto.DoctorId, cancellationToken);
            if (!doctorExists)
                throw new AppException(404, "Doctor not found.");

            var clinicExists = await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .AnyAsync(c => c.Id == dto.ClinicId, cancellationToken);
            if (!clinicExists)
                throw new AppException(404, "Clinic not found.");

            var exists = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .AnyAsync(dc => dc.DoctorId == dto.DoctorId && dc.ClinicId == dto.ClinicId, cancellationToken);
            if (exists)
                throw new AppException(409, "Doctor is already assigned to this clinic.");

            if (dto.IsPrimaryClinic)
            {
                var primaryExists = await _unitOfWork.Repository<DoctorClinic>().Query()
                    .AsNoTracking()
                    .AnyAsync(dc => dc.DoctorId == dto.DoctorId && dc.IsPrimaryClinic, cancellationToken);
                if (primaryExists)
                    throw new AppException(400, "Doctor already has a primary clinic. Set the current primary to false first.");
            }

            var doctorClinic = _mapper.Map<DoctorClinic>(dto);
            doctorClinic.JoinedAtUtc = DateTime.UtcNow;
            doctorClinic.IsActive = true;

            await _unitOfWork.Repository<DoctorClinic>().AddAsync(doctorClinic, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var result = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .Include(dc => dc.Clinic)
                .FirstAsync(dc => dc.Id == doctorClinic.Id, cancellationToken);
            return _mapper.Map<DoctorClinicDto>(result);
        }

        public async Task UnassignClinicAsync(int doctorClinicId, CancellationToken cancellationToken = default)
        {
            var dc = await _unitOfWork.Repository<DoctorClinic>().Query()
                .FirstOrDefaultAsync(x => x.Id == doctorClinicId, cancellationToken);
            if (dc is null)
                throw new AppException(404, "Doctor-clinic assignment not found.");

            var hasActiveAppointments = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .AnyAsync(a => a.DoctorClinicId == doctorClinicId &&
                    ClinicRules.IsBlockingStatus(a.Status), cancellationToken);

            if (hasActiveAppointments)
                throw new AppException(409, "Cannot unassign doctor from clinic: there are active appointments.");

            _unitOfWork.Repository<DoctorClinic>().Delete(dc);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DoctorClinicDto>> GetDoctorClinicsAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var doctorExists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == doctorId, cancellationToken);
            if (!doctorExists)
                throw new AppException(404, "Doctor not found.");

            var items = await _unitOfWork.Repository<DoctorClinic>().Query()
                .AsNoTracking()
                .Include(dc => dc.Clinic)
                .Where(dc => dc.DoctorId == doctorId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<DoctorClinicDto>>(items);
        }

        public async Task<int?> GetDoctorIdForCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                return null;

            var staff = await _unitOfWork.Repository<Staff>().Query()
                .AsNoTracking()
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);

            return staff?.Doctor?.Id;
        }
    }
}






