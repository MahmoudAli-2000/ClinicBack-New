using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasIndex(d => d.PublicCode)
                .IsUnique()
                .HasFilter("[PublicCode] <> ''")
                .HasDatabaseName("UX_Doctors_PublicCode");
            builder.HasOne(d => d.Specialty)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}