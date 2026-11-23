using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using TourSite.Core;
using TourSite.Core.DTOs.UserAuth;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IPublicCodeGenerator _publicCodeGenerator;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            IConfiguration configuration,
            IPublicCodeGenerator publicCodeGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _configuration = configuration;
            _publicCodeGenerator = publicCodeGenerator;
        }

        public async Task<UserAuthDto> RegisterAsync(RegisterAuthDto dto, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
                throw new AppException(409, "Email is already registered.");

            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.Phone,
                EmailConfirmed = false,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.Patient);
            if (!roleResult.Succeeded)
                throw new AppException(500, "Unable to assign the patient role.");

            var fullName = dto.FullName.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
                throw new AppException(400, "Full name is required.");

            var now = DateTime.UtcNow;
            var patientCountToday = await _unitOfWork.Repository<Patient>().Query()
                .CountAsync(p => p.CreatedAtUtc >= now.Date, cancellationToken);
            var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var patient = new Patient
            {
                UserId = user.Id,
                PatientNo = $"P{now:yyyyMMdd}{patientCountToday + 1:D4}",
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : "Patient",
                Phone = dto.Phone,
                Email = dto.Email,
                Status = PatientStatus.Active,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.Repository<Patient>().AddAsync(patient, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            patient.PublicCode = _publicCodeGenerator.Generate(EntityCodeType.Patient, patient.Id, now.Year);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await BuildAuthResultAsync(user, ipAddress, cancellationToken);
        }

        public async Task<UserAuthDto> LoginAsync(LoginAuthDto dto, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null || !user.IsActive)
                throw new AppException(401, "Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                throw new AppException(401, "Invalid email or password.");

            return await BuildAuthResultAsync(user, ipAddress, cancellationToken);
        }

        public async Task<UserAuthDto> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var tokenHash = _tokenService.HashToken(refreshToken);
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == tokenHash));

            if (user is null)
                throw new AppException(401, "Invalid refresh token.");

            var storedToken = user.RefreshTokens.First(t => t.TokenHash == tokenHash);
            if (!storedToken.IsActive)
                throw new AppException(401, "Refresh token has expired or been revoked.");

            storedToken.RevokedAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<RefreshToken>().Update(storedToken);

            return await BuildAuthResultAsync(user, ipAddress, cancellationToken);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var tokenHash = _tokenService.HashToken(refreshToken);
            var rt = await _unitOfWork.Repository<RefreshToken>()
                .Query()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (rt is null)
                throw new AppException(404, "Refresh token not found.");

            rt.RevokedAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<RefreshToken>().Update(rt);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return;

            var token = await _userManager.GenerateUserTokenAsync(user, "Default", "PasswordReset");
            var callbackUrl = $"{(_configuration["FrontendUrl"] ?? _configuration["BaseUrl"])}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendAsync(
                email,
                "Password Reset",
                $"<p>Please reset your password by clicking <a href=\"{callbackUrl}\">here</a>.</p>",
                cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                throw new AppException(404, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AppException(404, "User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AppException(404, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task ResendConfirmationAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || user.EmailConfirmed)
                return;

            var token = await _userManager.GenerateUserTokenAsync(user, "Default", "EmailConfirmation");
            var callbackUrl = $"{(_configuration["FrontendUrl"] ?? _configuration["BaseUrl"])}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendAsync(
                email,
                "Email Confirmation",
                $"<p>Please confirm your email by clicking <a href=\"{callbackUrl}\">here</a>.</p>",
                cancellationToken);
        }

        public async Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .Include(u => u.Patient)
                .Include(u => u.Staff)
                .ThenInclude(s => s!.Doctor)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                throw new AppException(404, "User not found.");

            var roles = (await _userManager.GetRolesAsync(user)).ToArray();

            return new CurrentUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles,
                PatientId = user.Patient?.Id,
                StaffId = user.Staff?.Id,
                DoctorId = user.Staff?.Doctor?.Id
            };
        }

        public async Task<CurrentUserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AppException(404, "User not found.");

            user.FullName = dto.FullName;
            if (dto.PhoneNumber is not null)
                user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new AppException(400, string.Join("; ", result.Errors.Select(e => e.Description)));

            return await GetCurrentUserAsync(userId, cancellationToken);
        }

        private async Task<UserAuthDto> BuildAuthResultAsync(User user, string? ipAddress, CancellationToken cancellationToken)
        {
            var (accessToken, expiresAt) = await _tokenService.CreateTokenAsync(user, _userManager);
            var roles = await _userManager.GetRolesAsync(user);

            var rawRefreshToken = GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(rawRefreshToken),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenDays),
                CreatedByIp = ipAddress,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new UserAuthDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                AccessTokenExpiresAtUtc = expiresAt,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.ToArray()
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}


