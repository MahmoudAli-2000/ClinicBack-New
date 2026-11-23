using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    [Index(nameof(DoctorId), nameof(ClinicId), IsUnique = true)]
    public class DoctorClinic
    {
        [Key]
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public int ClinicId { get; set; }

        public bool IsPrimaryClinic { get; set; } = false;

        [Column(TypeName = "decimal(12,2)")]
        public decimal? ConsultationFee { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(ClinicId))]
        public Clinic Clinic { get; set; } = null!;

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
