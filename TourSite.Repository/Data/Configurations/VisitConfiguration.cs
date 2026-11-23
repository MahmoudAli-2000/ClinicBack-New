using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class VisitConfiguration : IEntityTypeConfiguration<Visit>
    {
        public void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(v => v.DoctorNotes).IsUnicode();
        }
    }
}