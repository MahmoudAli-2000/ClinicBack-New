using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class VitalSignConfiguration : IEntityTypeConfiguration<VitalSign>
    {
        public void Configure(EntityTypeBuilder<VitalSign> builder)
        {
            builder.HasIndex(v => v.VisitId);
            builder.HasOne(v => v.Visit)
                .WithMany(x => x.VitalSigns)
                .HasForeignKey(v => v.VisitId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(v => v.RecordedByStaff)
                .WithMany(s => s.RecordedVitalSigns)
                .HasForeignKey(v => v.RecordedByStaffId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}