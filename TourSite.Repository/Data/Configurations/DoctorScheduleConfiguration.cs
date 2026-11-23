using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.HasIndex(s => new { s.DoctorClinicId, s.DayOfWeek });
            builder.HasOne(s => s.DoctorClinic)
                .WithMany(dc => dc.Schedules)
                .HasForeignKey(s => s.DoctorClinicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}