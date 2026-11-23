using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Repository.Data.Configurations
{
    public class DoctorTimeOffConfiguration : IEntityTypeConfiguration<DoctorTimeOff>
    {
        public void Configure(EntityTypeBuilder<DoctorTimeOff> builder)
        {
            builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(t => new { t.DoctorId, t.StartAtUtc, t.EndAtUtc });
            builder.HasOne(t => t.Doctor)
                .WithMany(d => d.TimeOffs)
                .HasForeignKey(t => t.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(t => t.Clinic)
                .WithMany(c => c.DoctorTimeOffs)
                .HasForeignKey(t => t.ClinicId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedDoctorTimeOffs)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}