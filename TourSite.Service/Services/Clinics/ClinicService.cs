using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using TourSite.Core;
using TourSite.Core.DTOs.Clinics;
using TourSite.Core.DTOs.Specialties;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Specification;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.Service.Services.Clinics
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public SpecialtyService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task<SpecialtyDto> CreateAsync(SpecialtyCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Repository<Specialty>().Query()
                .AsNoTracking()
                .AnyAsync(s => s.SpecialtyName.ToLower() == dto.SpecialtyName.ToLower(), cancellationToken))
                throw new AppException(409, "Specialty already exists.");

            var specialty = _mapper.Map<Specialty>(dto);
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/specialties");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                specialty.ImageCover = $"images/specialties/{fileName}";
            }
            specialty.IsActive = true;
            specialty.CreatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Specialty>().AddAsync(specialty, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task<SpecialtyDto> UpdateAsync(int id, SpecialtyUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var specialty = await _unitOfWork.Repository<Specialty>().Query()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (specialty is null)
                throw new AppException(404, "Specialty not found.");

            if (await _unitOfWork.Repository<Specialty>().Query()
                .AsNoTracking()
                .AnyAsync(s => s.Id != id && s.SpecialtyName.ToLower() == dto.SpecialtyName.ToLower(), cancellationToken))
                throw new AppException(409, "Another specialty with this name already exists.");

            specialty.SpecialtyName = dto.SpecialtyName;
            specialty.Description = dto.Description;
            specialty.IsActive = dto.IsActive;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/specialties");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                specialty.ImageCover = $"images/specialties/{fileName}";
            }
            else if (dto.ImageCover is not null)
                specialty.ImageCover = dto.ImageCover;

            _unitOfWork.Repository<Specialty>().Update(specialty);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var specialty = await _unitOfWork.Repository<Specialty>().Query()
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (specialty is null)
                throw new AppException(404, "Specialty not found.");

            if (specialty.Doctors.Any())
                throw new AppException(409, "Cannot delete specialty: doctors are assigned to it.");

            _unitOfWork.Repository<Specialty>().Delete(specialty);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<SpecialtyDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var spec = new BaseSpecifications<Specialty>(s => s.Id == id);
            var specialty = await _unitOfWork.Repository<Specialty>().GetByIdSpecAsync(spec);
            if (specialty is null)
                throw new AppException(404, "Specialty not found.");
            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task<PageinationResponse<SpecialtyDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Specialty>(string.IsNullOrEmpty(searchLower)
                ? null!
                : (s => EF.Functions.Like(s.SpecialtyName.ToLower(), $"%{searchLower}%")));

            if (string.IsNullOrEmpty(searchLower))
                spec = new BaseSpecifications<Specialty>();

            spec.AddOrderBy(s => s.SpecialtyName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Specialty>().GetAllSpecAsync(spec);
            var count = await _unitOfWork.Repository<Specialty>().GetCountAsync(spec);

            return new PageinationResponse<SpecialtyDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<SpecialtyDto>>(data));
        }
    }

    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ClinicService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task<ClinicDto> CreateAsync(ClinicCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .AnyAsync(c => c.ClinicCode == dto.ClinicCode, cancellationToken))
                throw new AppException(409, "A clinic with this code already exists.");

            var clinic = _mapper.Map<Clinic>(dto);
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/clinics");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                clinic.ImageCover = $"images/clinics/{fileName}";
            }
            clinic.IsActive = true;
            clinic.CreatedAtUtc = DateTime.UtcNow;
            clinic.UpdatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Clinic>().AddAsync(clinic, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<ClinicDto>(clinic);
        }

        public async Task<ClinicDto> UpdateAsync(int id, ClinicUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var clinic = await _unitOfWork.Repository<Clinic>().Query()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (clinic is null)
                throw new AppException(404, "Clinic not found.");

            if (await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .AnyAsync(c => c.Id != id && c.ClinicName.ToLower() == dto.ClinicName.ToLower(), cancellationToken))
                throw new AppException(409, "Another clinic with this name already exists.");

            clinic.ClinicName = dto.ClinicName;
            clinic.Phone = dto.Phone;
            clinic.Email = dto.Email;
            clinic.AddressLine1 = dto.AddressLine1;
            clinic.AddressLine2 = dto.AddressLine2;
            clinic.City = dto.City;
            clinic.Country = dto.Country;
            clinic.WorkingHoursNotes = dto.WorkingHoursNotes;
            clinic.IsActive = dto.IsActive;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/clinics");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                clinic.ImageCover = $"images/clinics/{fileName}";
            }
            else if (dto.ImageCover is not null)
                clinic.ImageCover = dto.ImageCover;
            clinic.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Clinic>().Update(clinic);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<ClinicDto>(clinic);
        }

        public async Task<ClinicDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var clinic = await _unitOfWork.Repository<Clinic>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (clinic is null)
                throw new AppException(404, "Clinic not found.");
            return _mapper.Map<ClinicDto>(clinic);
        }

        public async Task<PageinationResponse<ClinicDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Clinic>(string.IsNullOrEmpty(searchLower)
                ? null!
                : (c => EF.Functions.Like(c.ClinicName.ToLower(), $"%{searchLower}%") ||
                      EF.Functions.Like(c.ClinicCode.ToLower(), $"%{searchLower}%") ||
                      c.City != null && EF.Functions.Like(c.City.ToLower(), $"%{searchLower}%")));
            spec.AddOrderBy(c => c.ClinicName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Clinic>().GetAllSpecAsync(spec);
            var count = await _unitOfWork.Repository<Clinic>().GetCountAsync(spec);

            return new PageinationResponse<ClinicDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<ClinicDto>>(data));
        }

        public async Task SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default)
        {
            var clinic = await _unitOfWork.Repository<Clinic>().Query()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (clinic is null)
                throw new AppException(404, "Clinic not found.");

            clinic.IsActive = isActive;
            clinic.UpdatedAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<Clinic>().Update(clinic);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}






