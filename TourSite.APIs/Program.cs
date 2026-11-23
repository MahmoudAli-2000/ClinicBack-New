using Microsoft.EntityFrameworkCore;
using TourSite.APIs.Errors;
using TourSite.APIs.Helper;
using TourSite.APIs.MidleWare;
using TourSite.Core;
using TourSite.Core.Mapping;
using TourSite.Repository.Data.Contexts;

namespace TourSite.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OpenCors", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddLocalization();
            builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options => {
              var cultures = new[] { "en", "ar" };
              options.SetDefaultCulture("en").AddSupportedCultures(cultures).AddSupportedUICultures(cultures);
              options.ApplyCurrentCultureToResponseHeaders = true;
              options.RequestCultureProviders = new Microsoft.AspNetCore.Localization.IRequestCultureProvider[] { new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider(), new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider() };
            });
            builder.Services.AddControllers(o => o.Filters.Add<TourSite.APIs.Helper.LocalizedMessageFilter>()).AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDependency(builder.Configuration);

            var app = builder.Build();
            app.UseRequestLocalization();
            await app.UseConfigurationMiddleWare();

            app.UseRouting();
            app.UseCors("OpenCors");

            if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI();


            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
