using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Hosting;
using TourSite.Core;
using TourSite.Core.DTOs.Medications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Helper;
using TourSite.Core.Specification;
using TourSite.Core.Servicies.Contract;

namespace TourSite.Service.Services.Medications
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public MedicationService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task<MedicationDto> CreateAsync(MedicationCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(dto.MedicationCode) &&
                await _unitOfWork.Repository<Medication>().Query()
                    .AsNoTracking()
                    .AnyAsync(m => m.MedicationCode == dto.MedicationCode, cancellationToken))
                throw new AppException(409, "A medication with this code already exists.");

            var medication = _mapper.Map<Medication>(dto);
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/medications");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                medication.ImageCover = $"images/medications/{fileName}";
            }
            medication.IsActive = true;
            medication.CreatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.Repository<Medication>().AddAsync(medication, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<MedicationDto>(medication);
        }

        public async Task<MedicationDto> UpdateAsync(int id, MedicationUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var medication = await _unitOfWork.Repository<Medication>().Query()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (medication is null)
                throw new AppException(404, "Medication not found.");

            medication.MedicationCode = dto.MedicationCode;
            medication.BrandName = dto.BrandName;
            medication.GenericName = dto.GenericName;
            medication.GenericNameAr = dto.GenericNameAr;
            medication.BrandNameAr = dto.BrandNameAr;
            medication.DosageFormAr = dto.DosageFormAr;
            medication.DosageForm = dto.DosageForm;
            medication.Strength = dto.Strength;
            medication.Unit = dto.Unit;
            medication.IsActive = dto.IsActive;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/medications");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                medication.ImageCover = $"images/medications/{fileName}";
            }
            else if (dto.ImageCover is not null)
                medication.ImageCover = dto.ImageCover;

            _unitOfWork.Repository<Medication>().Update(medication);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<MedicationDto>(medication);
        }

        public async Task<MedicationDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var medication = await _unitOfWork.Repository<Medication>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (medication is null)
                throw new AppException(404, "Medication not found.");
            return _mapper.Map<MedicationDto>(medication);
        }

        public async Task<PageinationResponse<MedicationDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Medication>(string.IsNullOrEmpty(searchLower)
                ? null!
                : (m => (EF.Functions.Like(m.GenericName.ToLower(), $"%{searchLower}%") || (m.GenericNameAr != null && EF.Functions.Like(m.GenericNameAr, $"%{searchLower}%"))) ||
                       (m.BrandName != null && EF.Functions.Like(m.BrandName.ToLower(), $"%{searchLower}%")) ||
                       (m.MedicationCode != null && EF.Functions.Like(m.MedicationCode.ToLower(), $"%{searchLower}%"))));

            if (string.IsNullOrEmpty(searchLower))
                spec = new BaseSpecifications<Medication>();

            spec.AddOrderBy(m => m.GenericName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Medication>().GetAllSpecAsync(spec);
            var count = await _unitOfWork.Repository<Medication>().GetCountAsync(spec);

            return new PageinationResponse<MedicationDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<MedicationDto>>(data));
        }

        public async Task SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default)
        {
            var medication = await _unitOfWork.Repository<Medication>().Query()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (medication is null)
                throw new AppException(404, "Medication not found.");

            if (!isActive)
            {
                var hasPrescriptions = await _unitOfWork.Repository<PrescriptionItem>().Query()
                    .AsNoTracking()
                    .AnyAsync(pi => pi.MedicationId == id, cancellationToken);
                if (hasPrescriptions)
                    throw new AppException(409, "Cannot deactivate medication: it is referenced in prescriptions.");
            }

            medication.IsActive = isActive;
            _unitOfWork.Repository<Medication>().Update(medication);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}







