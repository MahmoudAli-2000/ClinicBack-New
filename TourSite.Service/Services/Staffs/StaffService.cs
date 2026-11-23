using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Staff;
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

namespace TourSite.Service.Services.Staffs
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPublicCodeGenerator _codeGenerator;
        private readonly IWebHostEnvironment _env;

        public StaffService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IPublicCodeGenerator codeGenerator, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _userManager = userManager;
            _roleManager = roleManager;
            _codeGenerator = codeGenerator;
            _env = env;
        }

        public async Task<StaffDto> CreateAsync(StaffCreateDto dto, CancellationToken cancellationToken = default)
        {
            //if (!_currentUser.IsFrontDesk)
            //    throw new AppException(403, "Only administrative/front-desk staff can create staff records.");

            var employeeNo = await GenerateEmployeeNumberAsync(cancellationToken);

            var staff = _mapper.Map<Staff>(dto);
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/staffs");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                staff.ImageCover = $"images/staffs/{fileName}";
            }
            staff.EmployeeNo = employeeNo;
            staff.CreatedAtUtc = DateTime.UtcNow;
            staff.UpdatedAtUtc = DateTime.UtcNow;
            staff.Status = StaffStatus.Active;

            if (string.IsNullOrWhiteSpace(staff.Phone))
                staff.Phone = dto.Phone;
            if (string.IsNullOrWhiteSpace(staff.Email))
                staff.Email = dto.Email;

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
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));

                if (dto.Roles?.Any() == true)
                    await _userManager.AddToRolesAsync(user, dto.Roles);

                staff.UserId = user.Id;
            }

            await _unitOfWork.Repository<Staff>().AddAsync(staff, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            staff.PublicCode = _codeGenerator.Generate(ResolveCodeType(dto.Roles), staff.Id, staff.CreatedAtUtc.Year);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await MapStaffDtoAsync(staff, cancellationToken);
        }

        public async Task<StaffDto> UpdateAsync(int id, StaffUpdateDto dto, CancellationToken cancellationToken = default)
        {
            //if (!_currentUser.IsFrontDesk)
            //    throw new AppException(403, "Only administrative/front-desk staff can update staff records.");

            var staff = await _unitOfWork.Repository<Staff>().Query()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (staff is null)
                throw new AppException(404, "Staff not found.");

            staff.FirstName = dto.FirstName;
            staff.LastName = dto.LastName;
            staff.Phone = dto.Phone;
            staff.Email = dto.Email;
            staff.PrimaryClinicId = dto.PrimaryClinicId;
            staff.JobTitle = dto.JobTitle;
            staff.Status = dto.Status;
            staff.HireDate = dto.HireDate;
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images/staffs");
                Directory.CreateDirectory(uploadDir);
                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(1600, 900), Mode = ResizeMode.Max }));
                    await image.SaveAsync(fullPath, new WebpEncoder() { Quality = 80 });
                }
                staff.ImageCover = $"images/staffs/{fileName}";
            }
            else if (dto.ImageCover is not null)
                staff.ImageCover = dto.ImageCover;
            staff.UpdatedAtUtc = DateTime.UtcNow;

            if (dto.Roles?.Any() == true)
            {
                var user = staff.UserId != null
                    ? await _userManager.FindByIdAsync(staff.UserId)
                    : null;

                if (user != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, dto.Roles);
                }
            }

            _unitOfWork.Repository<Staff>().Update(staff);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await MapStaffDtoAsync(staff, cancellationToken);
        }

        public async Task<StaffDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var staff = await _unitOfWork.Repository<Staff>().Query()
                .AsNoTracking()
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (staff is null)
                throw new AppException(404, "Staff not found.");

            return await MapStaffDtoAsync(staff, cancellationToken);
        }

        public async Task<PageinationResponse<StaffDto>> SearchAsync(StaffQueryDto query, CancellationToken cancellationToken = default)
        {
            var searchLower = query.SearchLower;
            var spec = new BaseSpecifications<Staff>(s =>
                (string.IsNullOrEmpty(searchLower) ||
                 s.FirstName.ToLower().Contains(searchLower) ||
                 s.LastName.ToLower().Contains(searchLower) ||
                 s.Phone != null && s.Phone.Contains(searchLower) ||
                 s.Email != null && s.Email.ToLower().Contains(searchLower)) &&
                (!query.Status.HasValue || s.Status == query.Status.Value) &&
                (!query.ClinicId.HasValue || s.PrimaryClinicId == query.ClinicId.Value));

            spec.AddOrderBy(s => s.LastName);
            spec.ApplyPag(query.SafePageSize, query.SafePageIndex);

            var data = await _unitOfWork.Repository<Staff>().GetAllSpecAsync(spec);
            var count = await _unitOfWork.Repository<Staff>().GetCountAsync(spec);

            var dtos = new List<StaffDto>();
            foreach (var staff in data)
                dtos.Add(await MapStaffDtoAsync(staff, cancellationToken));

            return new PageinationResponse<StaffDto>(query.SafePageIndex, query.SafePageSize, count, dtos);
        }

        public async Task SetStatusAsync(int id, StaffStatus status, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsFrontDesk)
                throw new AppException(403, "Only administrative/front-desk staff can update staff status.");

            var staff = await _unitOfWork.Repository<Staff>().Query()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (staff is null)
                throw new AppException(404, "Staff not found.");

            staff.Status = status;
            staff.UpdatedAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<Staff>().Update(staff);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        private async Task<StaffDto> MapStaffDtoAsync(Staff staff, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<StaffDto>(staff);
            if (staff.UserId != null)
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(staff.UserId));
                dto.Roles = roles.ToArray();
            }
            return dto;
        }

        private async Task<string> GenerateEmployeeNumberAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var count = await _unitOfWork.Repository<Staff>().Query()
                .CountAsync(s => s.CreatedAtUtc >= today, cancellationToken);
            return $"E{today:yyyyMMdd}{count + 1:D4}";
        }

        private static EntityCodeType ResolveCodeType(IReadOnlyCollection<string>? roles)
        {
            if (roles is null || roles.Count == 0)
                return EntityCodeType.Staff;

            var isAdministrative = roles.Any(role => role is AppRoles.Admin or AppRoles.Manager or AppRoles.Receptionist);
            return isAdministrative ? EntityCodeType.Admin : EntityCodeType.Staff;
        }
    }
}






