using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
    {
        public void Configure(EntityTypeBuilder<Diagnosis> builder)
        {
            builder.Property(d => d.DiagnosisType).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(d => d.VisitId);
            builder.HasOne(d => d.Visit)
                .WithMany(v => v.Diagnoses)
                .HasForeignKey(d => d.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(d => d.DiagnosedByDoctor)
                .WithMany(x => x.Diagnoses)
                .HasForeignKey(d => d.DiagnosedByDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}