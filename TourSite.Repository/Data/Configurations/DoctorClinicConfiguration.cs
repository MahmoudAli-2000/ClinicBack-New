using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class DoctorClinicConfigurations : IEntityTypeConfiguration<DoctorClinic>
    {
        public void Configure(EntityTypeBuilder<DoctorClinic> builder)
        {
            builder.HasOne(dc => dc.Doctor)
                .WithMany(d => d.DoctorClinics)
                .HasForeignKey(dc => dc.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dc => dc.Clinic)
                .WithMany(c => c.DoctorClinics)
                .HasForeignKey(dc => dc.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}