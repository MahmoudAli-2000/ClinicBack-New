using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(p => p.VisitId);
            builder.HasIndex(p => p.PrescribedByDoctorId);
            builder.HasOne(p => p.Visit)
                .WithMany(v => v.Prescriptions)
                .HasForeignKey(p => p.VisitId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.PrescribedByDoctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.PrescribedByDoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}