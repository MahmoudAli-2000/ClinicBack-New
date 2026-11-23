using AutoMapper;
using TourSite.Core.DTOs.Appointments;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
           CreateMap<Appointment, AppointmentDto>()
                .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient == null
                    ? string.Empty
                    : s.Patient.FirstName + " " + s.Patient.LastName))
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.DoctorClinic == null || s.DoctorClinic.Doctor == null || s.DoctorClinic.Doctor.Staff == null
                    ? string.Empty
                    : s.DoctorClinic.Doctor.Staff.FirstName + " " + s.DoctorClinic.Doctor.Staff.LastName))
                .ForMember(d => d.ClinicName, o => o.MapFrom(s => s.DoctorClinic == null || s.DoctorClinic.Clinic == null
                    ? string.Empty
                    : s.DoctorClinic.Clinic.ClinicName))
                .ForMember(d => d.VisitId, o => o.MapFrom(s => s.Visit == null ? (int?)null : s.Visit.Id))
                .ForMember(d => d.HasReview, o => o.MapFrom(s => s.Review != null));
        }
    }
}