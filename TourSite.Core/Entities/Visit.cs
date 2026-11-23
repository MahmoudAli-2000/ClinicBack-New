using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    [Index(nameof(AppointmentId), IsUnique = true)]
    public class Visit
    {
        [Key]
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public DateTime? CheckInAtUtc { get; set; }
        public DateTime? VisitStartAtUtc { get; set; }
        public DateTime? VisitEndAtUtc { get; set; }

        [MaxLength(1500)]
        public string? ChiefComplaint { get; set; }

        public string? DoctorNotes { get; set; }

        [Required, MaxLength(20)]
        public VisitStatus Status { get; set; } 

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;

        public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
