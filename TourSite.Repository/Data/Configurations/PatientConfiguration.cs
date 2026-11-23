using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Repository.Data.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.Gender).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(p => p.Phone);
            builder.HasIndex(p => p.Email);
            builder.HasIndex(p => new { p.LastName, p.FirstName });
            builder.HasIndex(p => p.PublicCode)
                .IsUnique()
                .HasFilter("[PublicCode] <> ''")
                .HasDatabaseName("UX_Patients_PublicCode");
        }
    }
}