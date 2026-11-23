using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Repository.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(s => s.PublicCode)
                .IsUnique()
                .HasFilter("[PublicCode] <> ''")
                .HasDatabaseName("UX_StaffMembers_PublicCode");
            builder.HasOne(s => s.PrimaryClinic)
                .WithMany(c => c.StaffMembers)
                .HasForeignKey(s => s.PrimaryClinicId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.Doctor)
                .WithOne(d => d.Staff)
                .HasForeignKey<Doctor>(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}