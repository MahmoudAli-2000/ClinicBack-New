using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using TourSite.Core;
using TourSite.Core.DTOs;
using TourSite.Core.DTOs.Patients;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Specification;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;
using TourSite.Repository.Repositories;


namespace TourSite.Service.Services.Patients
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly UserManager<User> _userManager;
        private readonly IPublicCodeGenerator _codeGenerator;
        private readonly IWebHostEnvironment _env;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser,
            UserManager<User> userManager, IPublicCodeGenerator codeGenerator, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _userManager = userManager;
            _codeGenerator = codeGenerator;
            _env = env;
        }

        public async Task<PatientDetailsDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
        {
            var patientNo = await GeneratePatientNumberAsync(cancellationToken);

            var patient = _mapper.Map<Patient>(dto);
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/patients");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                patient.ImageCover = $"images/patients/{fileName}";
            }
            patient.PatientNo = patientNo;
            patient.CreatedAtUtc = DateTime.UtcNow;
            patient.UpdatedAtUtc = DateTime.UtcNow;
            patient.Status = PatientStatus.Active;

            if (dto.CreateAccount && !string.IsNullOrWhiteSpace(dto.Password))
            {
                var email = dto.Email;
                if (string.IsNullOrWhiteSpace(email))
                    throw new AppException(400, "Email is required to create an account.");

                if (await _userManager.FindByEmailAsync(email) is not null)
                    throw new AppException(409, "A user with this email already exists.");

                var user = new User
                {
                    Email = email,
                    UserName = email,
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    PhoneNumber = dto.Phone,
                    EmailConfirmed = false,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, AppRoles.Patient);
                patient.UserId = user.Id;
            }

            await _unitOfWork.Repository<Patient>().AddAsync(patient, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            patient.PublicCode = _codeGenerator.Generate(EntityCodeType.Patient, patient.Id, patient.CreatedAtUtc.Year);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _mapper.Map<PatientDetailsDto>(patient);
            return _mapper.Map<PatientDetailsDto>(patient);
        }

        public async Task<PatientDetailsDto> UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await EnsureCanAccessPatientAsync(id, cancellationToken);

            var patient = await _unitOfWork.Repository<Patient>().Query()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;
            patient.Phone = dto.Phone;
            patient.Email = dto.Email;
            patient.AddressLine1 = dto.AddressLine1;
            patient.AddressLine2 = dto.AddressLine2;
            patient.City = dto.City;
            patient.EmergencyContactName = dto.EmergencyContactName;
            patient.EmergencyContactPhone = dto.EmergencyContactPhone;
            patient.Status = dto.Status;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/patients");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                patient.ImageCover = $"images/patients/{fileName}";
            }
            else if (dto.ImageCover is not null)
                patient.ImageCover = dto.ImageCover;
            patient.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<PatientDetailsDto>(patient);
        }

        public async Task<PatientDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await EnsureCanAccessPatientAsync(id, cancellationToken);

            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            return _mapper.Map<PatientDetailsDto>(patient);
        }

        public async Task<PageinationResponse<PatientListDto>> SearchAsync(PatientQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Patient>(p =>
                (string.IsNullOrEmpty(searchLower) ||
                 p.FirstName.ToLower().Contains(searchLower) ||
                 p.LastName.ToLower().Contains(searchLower) ||
                 p.Phone != null && p.Phone.Contains(searchLower) ||
                 p.Email != null && p.Email.ToLower().Contains(searchLower) ||
                 p.PatientNo.Contains(searchLower)) &&
                (!query.Status.HasValue || p.Status == query.Status.Value));

            spec.AddOrderBy(p => p.LastName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Patient>().GetAllSpecAsync(spec);
            var count = query.Status.HasValue
                ? await _unitOfWork.Repository<Patient>().Query()
                    .CountAsync(p => (string.IsNullOrEmpty(searchLower) ||
                        p.FirstName.ToLower().Contains(searchLower) ||
                        p.LastName.ToLower().Contains(searchLower) ||
                        p.Phone != null && p.Phone.Contains(searchLower) ||
                        p.Email != null && p.Email.ToLower().Contains(searchLower) ||
                        p.PatientNo.Contains(searchLower)) &&
                        p.Status == query.Status.Value, cancellationToken)
                : await _unitOfWork.Repository<Patient>().GetCountAsync(spec);

            return new PageinationResponse<PatientListDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<PatientListDto>>(data));
        }

        public async Task SetStatusAsync(int id, PatientStatus status, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Repository<Patient>().Query()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            patient.Status = status;
            patient.UpdatedAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task EnsureCanAccessPatientAsync(int patientId, CancellationToken cancellationToken = default)
        {
            if (_currentUser.IsAdmin || _currentUser.IsFrontDesk)
                return;

            if (_currentUser.IsPatient)
            {
                var patient = await _unitOfWork.Repository<Patient>().Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken);
                if (patient == null || patient.Id != patientId)
                    throw new AppException(403, "You do not have access to this patient's data.");
            }
            else if (_currentUser.IsClinicalStaff)
            {
                var exists = await _unitOfWork.Repository<Patient>().Query()
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == patientId, cancellationToken);
                if (!exists)
                    throw new AppException(404, "Patient not found.");
            }
            else
            {
                throw new AppException(403, "You do not have access to patient data.");
            }
        }

        private async Task<string> GeneratePatientNumberAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var count = await _unitOfWork.Repository<Patient>().Query()
                .CountAsync(p => p.CreatedAtUtc >= today, cancellationToken);
            return $"P{today:yyyyMMdd}{count + 1:D4}";
        }
    }
}




