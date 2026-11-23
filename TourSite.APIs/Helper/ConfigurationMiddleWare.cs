using Microsoft.EntityFrameworkCore;
using TourSite.APIs.MidleWare;
using TourSite.Core;
using TourSite.Repository.Data;
using TourSite.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using TourSite.Core.Entities;
using Microsoft.Extensions.Logging;

namespace TourSite.APIs.Helper
{
    public static class ConfigurationMiddleWare
    {
        public static async Task<WebApplication> UseConfigurationMiddleWare(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>();
            var loggerInstance = logger.CreateLogger<Program>();

            try
            {
                var context = services.GetRequiredService<TourDbContext>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var configuration = services.GetRequiredService<IConfiguration>();

                await context.Database.MigrateAsync();
                await ClinicDbContextSeed.SeedAsync(context, roleManager, userManager, configuration, loggerInstance);
            }
            catch (Exception ex)
            {
                loggerInstance.LogError(ex, "An error occurred while migrating or seeding the database.");
            }

            app.UseMiddleware<ExceptionMidleWare>();

            return app;
        }
    }
}
