using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Repository.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(a => a.BookingSource).HasConversion<string>().HasMaxLength(20);

            builder.HasIndex(a => a.PatientId);
            builder.HasIndex(a => a.DoctorClinicId);
            builder.HasIndex(a => a.ScheduledStartAtUtc);
            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => new { a.DoctorClinicId, a.ScheduledStartAtUtc });
            builder.HasIndex(a => new { a.PatientId, a.ScheduledStartAtUtc });
            // Concurrent exact-slot protection for active bookings. Overlaps with different start times
            // are additionally guarded in AppointmentService via a serializable transaction.
            builder.HasIndex(a => new { a.DoctorClinicId, a.ScheduledStartAtUtc })
                .IsUnique()
                .HasFilter("[Status] IN ('Scheduled','Confirmed','CheckedIn','InProgress')")
                .HasDatabaseName("UX_Appointments_DoctorClinic_Start_Active");

            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.DoctorClinic)
                .WithMany(dc => dc.Appointments)
                .HasForeignKey(a => a.DoctorClinicId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.CreatedBy)
                .WithMany(u => u.CreatedAppointments)
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.Visit)
                .WithOne(v => v.Appointment)
                .HasForeignKey<Visit>(v => v.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Review)
                .WithOne(r => r.Appointment)
                .HasForeignKey<Review>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}