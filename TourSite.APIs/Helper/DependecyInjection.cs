using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using TourSite.APIs.Errors;
using TourSite.Core;
using TourSite.Core.Mapping;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Entities;
using TourSite.Core.Helper;
using TourSite.Repository.Data.Contexts;
using TourSite.Repository.Repositories;
using TourSite.Service.Services.Auth;
using TourSite.Service.Services.Token;
using TourSite.Service.Services.Users;
using TourSite.Service.Services.Patients;
using TourSite.Service.Services.Staffs;
using TourSite.Service.Services.Doctors;
using TourSite.Service.Services.Clinics;
using TourSite.Service.Services.Schedules;
using TourSite.Service.Services.Appointments;
using TourSite.Service.Services.Visits;
using TourSite.Service.Services.Medications;
using TourSite.Service.Services.Prescriptions;
using TourSite.Service.Services.Reviews;
using TourSite.Service.Services.Notifications;
using TourSite.Service.Services.Emails;

namespace TourSite.APIs.Helper
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddDependency(this IServiceCollection services, IConfiguration configuration)
        {
            AddSwaggerService(services);
            AddDbContextService(services, configuration);
            AddUserDefinedService(services);
            AddMappingService(services, configuration);
            AddValidationErrorService(services);
            AddIdentityService(services);
            AddAuthenticationService(services, configuration);

            return services;
        }

        private static IServiceCollection AddSwaggerService(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        private static IServiceCollection AddDbContextService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TourDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));
            return services;
        }

        private static IServiceCollection AddUserDefinedService(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddScoped<INotificationChannelSender, InAppNotificationChannel>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IVisitService, VisitService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPublicCodeGenerator, PublicCodeGenerator>();
            return services;
        }

        private static IServiceCollection AddMappingService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(m => m.AddProfile(new ClinicProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new UserProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new PatientProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new StaffProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new DoctorProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new SpecialtyProfile()));
            services.AddAutoMapper(m => m.AddProfile(new TimeOffProfile()));
            services.AddAutoMapper(m => m.AddProfile(new VisitProfile()));
            services.AddAutoMapper(m => m.AddProfile(new VitalSignProfile()));
            services.AddAutoMapper(m => m.AddProfile(new DiagnosisProfile()));
            services.AddAutoMapper(m => m.AddProfile(new MedicationProfile(configuration)));
            services.AddAutoMapper(m => m.AddProfile(new PrescriptionProfile()));
            services.AddAutoMapper(m => m.AddProfile(new ReviewProfile()));
            services.AddAutoMapper(m => m.AddProfile(new NotificationProfile()));
            services.AddAutoMapper(m => m.AddProfile(new AppointmentProfile()));
            services.AddAutoMapper(m => m.AddProfile(new ScheduleProfile()));
            services.AddAutoMapper(m => m.AddProfile(new DoctorClinicProfile()));

            return services;
        }

        private static IServiceCollection AddValidationErrorService(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToArray();

                    var response = new ValidationErrorResponse
                    {
                        errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            return services;
        }

        private static IServiceCollection AddIdentityService(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<TourDbContext>()
             .AddDefaultTokenProviders();
            return services;
        }

        private static IServiceCollection AddAuthenticationService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    RoleClaimType = "role",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]
                            ?? throw new InvalidOperationException("JwtSettings:Key is missing")))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var cookieToken = context.Request.Cookies["jwt"];
                        if (!string.IsNullOrEmpty(cookieToken))
                            context.Token = cookieToken;

                        if (!string.IsNullOrEmpty(context.Token))
                            return Task.CompletedTask;

                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
