using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.IsActive).HasDefaultValue(true);

            builder.HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.Staff)
                .WithOne(s => s.User)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.NotificationPreference)
                .WithOne(p => p.User)
                .HasForeignKey<NotificationPreference>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}