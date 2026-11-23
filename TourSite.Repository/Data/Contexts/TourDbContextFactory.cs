using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TourSite.Repository.Data.Contexts
{
    public class TourDbContextFactory : IDesignTimeDbContextFactory<TourDbContext>
    {
        public TourDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("CLINIC_DB")
                ?? "Server=(localdb)\\mssqllocaldb;Database=ClinicDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<TourDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new TourDbContext(options);
        }
    }
}
