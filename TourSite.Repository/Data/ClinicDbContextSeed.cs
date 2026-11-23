using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TourSite.Core.Entities;
using TourSite.Core.Helper;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;
using TourSite.Repository.Data.Contexts;

namespace TourSite.Repository.Data
{
    public static class ClinicDbContextSeed
    {
        public static async Task SeedAsync(
            TourDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger logger)
        {
            await SeedRolesAsync(roleManager);

            var adminUser = await SeedAdminUserAsync(userManager, configuration, logger);

            await SeedClinicDataAsync(context, userManager, configuration, adminUser, logger);
            await SeedBilingualCatalogAsync(context);
        }

        public static async Task SeedBilingualCatalogAsync(TourDbContext context)
        {
            var specialties = new Dictionary<string, (string Name, string Description)> {
                ["General Practice"] = ("طب عام", "الرعاية الأولية وطب الأسرة"),
                ["Cardiology"] = ("أمراض القلب", "القلب والأوعية الدموية"),
                ["Pediatrics"] = ("طب الأطفال", "رعاية الرضع والأطفال والمراهقين"),
                ["Dermatology"] = ("الأمراض الجلدية", "البشرة والشعر والأظافر"),
                ["Orthopedics"] = ("جراحة العظام", "الجهاز العضلي والهيكلي والإصابات")
            };
            foreach (var row in await context.Specialties.ToListAsync())
                if (specialties.TryGetValue(row.SpecialtyName, out var text)) { row.SpecialtyNameAr ??= text.Name; row.DescriptionAr ??= text.Description; }
            foreach (var row in await context.Clinics.ToListAsync()) {
                row.ClinicNameAr ??= row.ClinicCode switch { "CL-DT" => "عيادة وسط البلد", "CL-RS" => "عيادة النيل", _ => null };
                if (row.ClinicCode is "CL-DT" or "CL-RS") row.WorkingHoursNotesAr ??= "الأحد إلى الخميس، من التاسعة صباحا حتى السادسة مساء";
            }
            var medications = new Dictionary<string,string> { ["Paracetamol"] = "باراسيتامول", ["Ibuprofen"] = "إيبوبروفين", ["Amoxicillin"] = "أموكسيسيلين", ["Amoxicillin/Clavulanate"] = "أموكسيسيلين / كلافولانات", ["Azithromycin"] = "أزيثروميسين", ["Metformin"] = "ميتفورمين", ["Amlodipine"] = "أملوديبين", ["Omeprazole"] = "أوميبرازول", ["Cetirizine"] = "سيتريزين", ["Salbutamol"] = "سالبوتامول" };
            foreach (var row in await context.Medications.ToListAsync()) {
                if (medications.TryGetValue(row.GenericName, out var text)) row.GenericNameAr ??= text;
                if (row.DosageForm == "Inhaler") row.DosageFormAr ??= "بخاخ";
                var brands = new Dictionary<string,string> { ["Panadol"]="بانادول", ["Augmentin"]="أوجمنتين", ["Norvasc"]="نورفاسك", ["Ventolin"]="فنتولين", ["Zithromax"]="زيثروماكس" };
                if(row.BrandName != null && brands.TryGetValue(row.BrandName,out var brand)) row.BrandNameAr ??= brand;
                if (row.DosageForm == "Tablet") row.DosageFormAr ??= "أقراص";
                if (row.DosageForm == "Capsule") row.DosageFormAr ??= "كبسولات";
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task<User?> SeedAdminUserAsync(
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger logger)
        {
            var adminEmail = configuration["Seed:AdminEmail"];
            var adminPassword = configuration["Seed:AdminPassword"];
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                return null;

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing is not null)
                return existing;

            var admin = new User
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = configuration["Seed:AdminFullName"] ?? "Clinic Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                logger.LogWarning("Could not seed development admin: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
                return null;
            }

            await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            return admin;
        }

        private static async Task SeedClinicDataAsync(
            TourDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            User? adminUser,
            ILogger logger)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var codeGenerator = new PublicCodeGenerator();
            var staffPassword = configuration["Seed:StaffPassword"] ?? "Password@123";
            var patientPassword = configuration["Seed:PatientPassword"] ?? "Password@123";

            // ---------------- Specialties ----------------
            if (!await context.Specialties.AnyAsync())
            {
                context.Specialties.AddRange(
                    new Specialty { SpecialtyName = "General Practice", Description = "Primary care and family medicine", CreatedAtUtc = now },
                    new Specialty { SpecialtyName = "Cardiology", Description = "Heart and vascular system", CreatedAtUtc = now },
                    new Specialty { SpecialtyName = "Pediatrics", Description = "Infants, children and adolescents", CreatedAtUtc = now },
                    new Specialty { SpecialtyName = "Dermatology", Description = "Skin, hair and nails", CreatedAtUtc = now },
                    new Specialty { SpecialtyName = "Orthopedics", Description = "Musculoskeletal system and injuries", CreatedAtUtc = now }
                );
                await context.SaveChangesAsync();
            }

            // ---------------- Clinics ----------------
            if (!await context.Clinics.AnyAsync())
            {
                context.Clinics.AddRange(
                    new Clinic
                    {
                        ClinicCode = "CL-DT",
                        ClinicName = "Downtown Clinic",
                        Phone = "+201000000101",
                        Email = "downtown@clinic.local",
                        AddressLine1 = "12 Tahrir Street",
                        City = "Cairo",
                        Country = "Egypt",
                        WorkingHoursNotes = "Sunday - Thursday, 9:00 AM - 6:00 PM",
                        IsActive = true,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Clinic
                    {
                        ClinicCode = "CL-RS",
                        ClinicName = "Riverside Clinic",
                        Phone = "+201000000102",
                        Email = "riverside@clinic.local",
                        AddressLine1 = "45 Corniche Street",
                        City = "Alexandria",
                        Country = "Egypt",
                        WorkingHoursNotes = "Sunday - Thursday, 4:00 PM - 10:00 PM",
                        IsActive = true,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                );
                await context.SaveChangesAsync();
            }

            var clinics = await context.Clinics.OrderBy(c => c.Id).ToListAsync();
            var specialties = await context.Specialties.OrderBy(s => s.Id).ToListAsync();

            // ---------------- Accounts ----------------
            var frontDeskUser = await EnsureUserAsync(userManager, "frontdesk@clinic.local", "Sara Mahmoud",
                staffPassword, AppRoles.Receptionist, "+201000000102", logger);
            var managerUser = await EnsureUserAsync(userManager, "manager@clinic.local", "Mona Adel",
                staffPassword, AppRoles.Manager, "+201000000103", logger);
            var nurseUser = await EnsureUserAsync(userManager, "nurse@clinic.local", "Hoda Kamel",
                staffPassword, AppRoles.Nurse, "+201000000104", logger);
            var doctorUserOne = await EnsureUserAsync(userManager, "ahmed.doctor@clinic.local", "Ahmed Fouad",
                staffPassword, AppRoles.Doctor, "+201000000105", logger);
            var doctorUserTwo = await EnsureUserAsync(userManager, "nour.doctor@clinic.local", "Nour Hassan",
                staffPassword, AppRoles.Doctor, "+201000000106", logger);

            // ---------------- Staff ----------------
            if (!await context.StaffMembers.AnyAsync())
            {
                var staff = new List<Staff>();

                if (adminUser is not null)
                    staff.Add(BuildStaff(adminUser, "EMP-0001", "Clinic", "Administrator", "admin@clinic.local",
                        "+201000000001", "Clinic Administrator", EntityCodeType.Admin, clinics[0].Id, today.AddYears(-3), now));

                if (frontDeskUser is not null)
                    staff.Add(BuildStaff(frontDeskUser, "EMP-0002", "Sara", "Mahmoud", "frontdesk@clinic.local",
                        "+201000000102", "Front Desk Receptionist", EntityCodeType.Admin, clinics[0].Id, today.AddYears(-2), now));

                if (managerUser is not null)
                    staff.Add(BuildStaff(managerUser, "EMP-0003", "Mona", "Adel", "manager@clinic.local",
                        "+201000000103", "Clinic Manager", EntityCodeType.Admin, clinics[0].Id, today.AddYears(-4), now));

                if (nurseUser is not null)
                    staff.Add(BuildStaff(nurseUser, "EMP-0004", "Hoda", "Kamel", "nurse@clinic.local",
                        "+201000000104", "Triage Nurse", EntityCodeType.Staff, clinics[0].Id, today.AddYears(-1), now));

                if (doctorUserOne is not null)
                    staff.Add(BuildStaff(doctorUserOne, "EMP-0005", "Ahmed", "Fouad", "ahmed.doctor@clinic.local",
                        "+201000000105", "Consultant Cardiologist", EntityCodeType.Staff, clinics[0].Id, today.AddYears(-5), now));

                if (doctorUserTwo is not null)
                    staff.Add(BuildStaff(doctorUserTwo, "EMP-0006", "Nour", "Hassan", "nour.doctor@clinic.local",
                        "+201000000106", "Consultant Pediatrician", EntityCodeType.Staff, clinics[1].Id, today.AddYears(-3), now));

                context.StaffMembers.AddRange(staff);
                await context.SaveChangesAsync();

                foreach (var member in staff)
                {
                    member.PublicCode = codeGenerator.Generate(
                        member.JobTitle is "Clinic Administrator" or "Clinic Manager" or "Front Desk Receptionist"
                            ? EntityCodeType.Admin
                            : EntityCodeType.Staff,
                        member.Id,
                        member.CreatedAtUtc.Year);
                }

                await context.SaveChangesAsync();
            }

            var staffMembers = await context.StaffMembers.OrderBy(s => s.Id).ToListAsync();

            // ---------------- Doctors ----------------
            if (!await context.Doctors.AnyAsync())
            {
                var newDoctors = new List<Doctor>();

                var cardiologistStaff = staffMembers.FirstOrDefault(s => s.Email == "ahmed.doctor@clinic.local");
                var pediatricianStaff = staffMembers.FirstOrDefault(s => s.Email == "nour.doctor@clinic.local");
                var cardiology = specialties.FirstOrDefault(s => s.SpecialtyName.Contains("Cardio", StringComparison.OrdinalIgnoreCase))
                                 ?? specialties[0];
                var pediatrics = specialties.FirstOrDefault(s => s.SpecialtyName.Contains("Pediatric", StringComparison.OrdinalIgnoreCase))
                                 ?? specialties[Math.Min(1, specialties.Count - 1)];

                if (cardiologistStaff is not null)
                    newDoctors.Add(new Doctor
                    {
                        StaffId = cardiologistStaff.Id,
                        SpecialtyId = cardiology.Id,
                        LicenseNo = "LIC-EG-1001",
                        ConsultationDurationMinutes = 30,
                        DefaultConsultationFee = 350m,
                        Bio = "Consultant cardiologist with 12 years of experience in interventional cardiology and heart failure management.",
                        IsActive = true,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    });

                if (pediatricianStaff is not null)
                    newDoctors.Add(new Doctor
                    {
                        StaffId = pediatricianStaff.Id,
                        SpecialtyId = pediatrics.Id,
                        LicenseNo = "LIC-EG-1002",
                        ConsultationDurationMinutes = 20,
                        DefaultConsultationFee = 250m,
                        Bio = "Pediatrician focused on newborn and childhood development, vaccination programs and nutrition.",
                        IsActive = true,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    });

                context.Doctors.AddRange(newDoctors);
                await context.SaveChangesAsync();

                foreach (var doctor in newDoctors)
                    doctor.PublicCode = codeGenerator.Generate(EntityCodeType.Doctor, doctor.Id, doctor.CreatedAtUtc.Year);

                await context.SaveChangesAsync();
            }

            var doctors = await context.Doctors.OrderBy(d => d.Id).ToListAsync();

            // ---------------- Doctor / Clinic assignments ----------------
            if (!await context.DoctorClinics.AnyAsync())
            {
                var assignments = new List<DoctorClinic>();

                for (var index = 0; index < doctors.Count; index++)
                {
                    var clinic = clinics[index % clinics.Count];
                    assignments.Add(new DoctorClinic
                    {
                        DoctorId = doctors[index].Id,
                        ClinicId = clinic.Id,
                        IsPrimaryClinic = true,
                        ConsultationFee = doctors[index].DefaultConsultationFee,
                        IsActive = true,
                        JoinedAtUtc = now
                    });
                }

                if (doctors.Count > 1)
                {
                    assignments.Add(new DoctorClinic
                    {
                        DoctorId = doctors[0].Id,
                        ClinicId = clinics[Math.Min(1, clinics.Count - 1)].Id,
                        IsPrimaryClinic = false,
                        ConsultationFee = 300m,
                        IsActive = true,
                        JoinedAtUtc = now
                    });
                }

                context.DoctorClinics.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            var doctorClinics = await context.DoctorClinics.OrderBy(dc => dc.Id).ToListAsync();

            // ---------------- Weekly schedules ----------------
            if (!await context.DoctorSchedules.AnyAsync())
            {
                var schedules = new List<DoctorSchedule>();

                foreach (var assignment in doctorClinics)
                {
                    schedules.Add(new DoctorSchedule
                    {
                        DoctorClinicId = assignment.Id,
                        DayOfWeek = 1,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        SlotDurationMinutes = 30,
                        EffectiveFrom = today.AddMonths(-1),
                        IsActive = true,
                        CreatedAtUtc = now
                    });

                    schedules.Add(new DoctorSchedule
                    {
                        DoctorClinicId = assignment.Id,
                        DayOfWeek = 3,
                        StartTime = new TimeSpan(12, 0, 0),
                        EndTime = new TimeSpan(20, 0, 0),
                        SlotDurationMinutes = 20,
                        EffectiveFrom = today.AddMonths(-1),
                        IsActive = true,
                        CreatedAtUtc = now
                    });
                }

                context.DoctorSchedules.AddRange(schedules);
                await context.SaveChangesAsync();
            }

            // ---------------- Time off ----------------
            if (!await context.DoctorTimeOffs.AnyAsync() && doctors.Count > 0)
            {
                context.DoctorTimeOffs.Add(new DoctorTimeOff
                {
                    DoctorId = doctors[0].Id,
                    ClinicId = clinics[0].Id,
                    StartAtUtc = today.AddDays(7).AddHours(9),
                    EndAtUtc = today.AddDays(7).AddHours(13),
                    Reason = "Scheduled cardiac surgery conference",
                    Status = TimeOffStatus.Approved,
                    CreatedByUserId = adminUser?.Id,
                    CreatedAtUtc = now
                });

                await context.SaveChangesAsync();
            }

            // ---------------- Patients ----------------
            var patientUserOne = await EnsureUserAsync(userManager, "patient.one@clinic.local", "Omar Tarek",
                patientPassword, AppRoles.Patient, "+201000000201", logger);
            var patientUserTwo = await EnsureUserAsync(userManager, "patient.two@clinic.local", "Laila Sherif",
                patientPassword, AppRoles.Patient, "+201000000202", logger);

            if (!await context.Patients.AnyAsync())
            {
                var newPatients = new List<Patient>
                {
                    new Patient
                    {
                        UserId = patientUserOne?.Id,
                        PatientNo = "PAT-0001",
                        FirstName = "Omar",
                        LastName = "Tarek",
                        DateOfBirth = today.AddYears(-34),
                        Gender = Gender.Male,
                        Phone = "+201000000201",
                        Email = "patient.one@clinic.local",
                        AddressLine1 = "8 Talaat Harb Street",
                        City = "Cairo",
                        EmergencyContactName = "Nour Tarek",
                        EmergencyContactPhone = "+201000000211",
                        Status = PatientStatus.Active,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Patient
                    {
                        UserId = patientUserTwo?.Id,
                        PatientNo = "PAT-0002",
                        FirstName = "Laila",
                        LastName = "Sherif",
                        DateOfBirth = today.AddYears(-28),
                        Gender = Gender.Female,
                        Phone = "+201000000202",
                        Email = "patient.two@clinic.local",
                        AddressLine1 = "21 Brazil Street",
                        City = "Alexandria",
                        EmergencyContactName = "Amr Sherif",
                        EmergencyContactPhone = "+201000000212",
                        Status = PatientStatus.Active,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Patient
                    {
                        PatientNo = "PAT-0003",
                        FirstName = "Youssef",
                        LastName = "Nabil",
                        DateOfBirth = today.AddYears(-46),
                        Gender = Gender.Male,
                        Phone = "+201000000203",
                        Email = "youssef.nabil@example.test",
                        AddressLine1 = "5 Ahmed Shawky Street",
                        City = "Cairo",
                        Status = PatientStatus.Active,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Patient
                    {
                        PatientNo = "PAT-0004",
                        FirstName = "Mariam",
                        LastName = "Emad",
                        DateOfBirth = today.AddYears(-8),
                        Gender = Gender.Female,
                        Phone = "+201000000204",
                        Email = "mariam.emad@example.test",
                        AddressLine1 = "17 Mohamed Farid Street",
                        City = "Cairo",
                        EmergencyContactName = "Hoda Emad",
                        EmergencyContactPhone = "+201000000214",
                        Status = PatientStatus.Active,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                };

                context.Patients.AddRange(newPatients);
                await context.SaveChangesAsync();

                foreach (var patient in newPatients)
                    patient.PublicCode = codeGenerator.Generate(EntityCodeType.Patient, patient.Id, patient.CreatedAtUtc.Year);

                await context.SaveChangesAsync();
            }

            var patients = await context.Patients.OrderBy(p => p.Id).ToListAsync();

            // ---------------- Medications ----------------
            if (!await context.Medications.AnyAsync())
            {
                context.Medications.AddRange(
                    new Medication { MedicationCode = "MED-001", BrandName = "Panadol", GenericName = "Paracetamol", DosageForm = "Tablet", Strength = "500 mg", Unit = "Tablet", IsActive = true, CreatedAtUtc = now },
                    new Medication { MedicationCode = "MED-002", BrandName = "Augmentin", GenericName = "Amoxicillin/Clavulanate", DosageForm = "Capsule", Strength = "1 g", Unit = "Capsule", IsActive = true, CreatedAtUtc = now },
                    new Medication { MedicationCode = "MED-003", BrandName = "Norvasc", GenericName = "Amlodipine", DosageForm = "Tablet", Strength = "5 mg", Unit = "Tablet", IsActive = true, CreatedAtUtc = now },
                    new Medication { MedicationCode = "MED-004", BrandName = "Ventolin", GenericName = "Salbutamol", DosageForm = "Inhaler", Strength = "100 mcg", Unit = "Inhaler", IsActive = true, CreatedAtUtc = now },
                    new Medication { MedicationCode = "MED-005", BrandName = "Zithromax", GenericName = "Azithromycin", DosageForm = "Tablet", Strength = "500 mg", Unit = "Tablet", IsActive = true, CreatedAtUtc = now }
                );

                await context.SaveChangesAsync();
            }

            var medications = await context.Medications.OrderBy(m => m.Id).ToListAsync();

            // ---------------- Appointments ----------------
            if (!await context.Appointments.AnyAsync() && patients.Count > 0 && doctorClinics.Count > 0)
            {
                var firstAssignment = doctorClinics[0];
                var secondAssignment = doctorClinics.Count > 1 ? doctorClinics[1] : doctorClinics[0];

                var newAppointments = new List<Appointment>
                {
                    new Appointment
                    {
                        PatientId = patients[0].Id,
                        DoctorClinicId = firstAssignment.Id,
                        ScheduledStartAtUtc = today.AddDays(1).AddHours(10),
                        ScheduledEndAtUtc = today.AddDays(1).AddHours(10.5),
                        Status = AppointmentStatus.Scheduled,
                        BookingSource = BookingSource.Patient,
                        Reason = "Chest pain and shortness of breath",
                        Notes = "First cardiology consultation",
                        CreatedByUserId = adminUser?.Id,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Appointment
                    {
                        PatientId = patients[1].Id,
                        DoctorClinicId = firstAssignment.Id,
                        ScheduledStartAtUtc = today.AddDays(1).AddHours(12),
                        ScheduledEndAtUtc = today.AddDays(1).AddHours(12.5),
                        Status = AppointmentStatus.Confirmed,
                        BookingSource = BookingSource.Receptionist,
                        Reason = "Follow-up check",
                        Notes = "Bring previous lab results",
                        CreatedByUserId = adminUser?.Id,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Appointment
                    {
                        PatientId = patients[2].Id,
                        DoctorClinicId = secondAssignment.Id,
                        ScheduledStartAtUtc = today.AddDays(-2).AddHours(11),
                        ScheduledEndAtUtc = today.AddDays(-2).AddHours(11.5),
                        Status = AppointmentStatus.Completed,
                        BookingSource = BookingSource.Admin,
                        Reason = "Hypertension review",
                        Notes = "Completed visit with prescription issued",
                        CreatedByUserId = adminUser?.Id,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Appointment
                    {
                        PatientId = patients[3].Id,
                        DoctorClinicId = secondAssignment.Id,
                        ScheduledStartAtUtc = today.AddDays(3).AddHours(15),
                        ScheduledEndAtUtc = today.AddDays(3).AddHours(15.33),
                        Status = AppointmentStatus.Cancelled,
                        BookingSource = BookingSource.Phone,
                        Reason = "Pediatric vaccination session",
                        Notes = "Cancelled by the patient",
                        CancellationReason = "Patient rescheduled the session",
                        CancelledAtUtc = now,
                        CreatedByUserId = adminUser?.Id,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                };

                context.Appointments.AddRange(newAppointments);
                await context.SaveChangesAsync();
            }

            var appointments = await context.Appointments.OrderBy(a => a.Id).ToListAsync();

            // ---------------- Visits ----------------
            if (!await context.Visits.AnyAsync() && appointments.Count > 0)
            {
                var completedAppointment = appointments.FirstOrDefault(a => a.Status == AppointmentStatus.Completed)
                                           ?? appointments[0];
                var confirmedAppointment = appointments.FirstOrDefault(a => a.Status == AppointmentStatus.Confirmed)
                                           ?? appointments[0];

                context.Visits.AddRange(
                    new Visit
                    {
                        AppointmentId = completedAppointment.Id,
                        CheckInAtUtc = completedAppointment.ScheduledStartAtUtc.AddMinutes(-10),
                        VisitStartAtUtc = completedAppointment.ScheduledStartAtUtc,
                        VisitEndAtUtc = completedAppointment.ScheduledEndAtUtc,
                        ChiefComplaint = "Persistent hypertension and mild headaches",
                        DoctorNotes = "Blood pressure controlled. Continue current therapy and re-check in one month.",
                        Status = VisitStatus.Completed,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    },
                    new Visit
                    {
                        AppointmentId = confirmedAppointment.Id,
                        CheckInAtUtc = confirmedAppointment.ScheduledStartAtUtc.AddMinutes(-5),
                        VisitStartAtUtc = confirmedAppointment.ScheduledStartAtUtc,
                        ChiefComplaint = "Routine follow-up",
                        DoctorNotes = "Patient checked in, awaiting the doctor.",
                        Status = VisitStatus.InProgress,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    }
                );

                await context.SaveChangesAsync();
            }

            var visits = await context.Visits.OrderBy(v => v.Id).ToListAsync();

            // ---------------- Vital signs ----------------
            if (!await context.VitalSigns.AnyAsync() && visits.Count > 0)
            {
                var nurseStaffId = staffMembers.FirstOrDefault(s => s.JobTitle == "Triage Nurse")?.Id;

                context.VitalSigns.AddRange(
                    new VitalSign
                    {
                        VisitId = visits[0].Id,
                        WeightKg = 84.5m,
                        HeightCm = 176m,
                        SystolicBP = 148,
                        DiastolicBP = 92,
                        HeartRateBpm = 78,
                        TemperatureC = 36.7m,
                        OxygenSaturationPct = 97.5m,
                        RespiratoryRate = 16,
                        RecordedByStaffId = nurseStaffId,
                        RecordedAtUtc = now,
                        Notes = "Measured on arrival"
                    },
                    new VitalSign
                    {
                        VisitId = visits.Count > 1 ? visits[1].Id : visits[0].Id,
                        WeightKg = 62.0m,
                        HeightCm = 165m,
                        SystolicBP = 118,
                        DiastolicBP = 76,
                        HeartRateBpm = 72,
                        TemperatureC = 36.4m,
                        OxygenSaturationPct = 98.2m,
                        RespiratoryRate = 15,
                        RecordedByStaffId = nurseStaffId,
                        RecordedAtUtc = now
                    }
                );

                await context.SaveChangesAsync();
            }

            // ---------------- Diagnoses ----------------
            if (!await context.Diagnoses.AnyAsync() && visits.Count > 0)
            {
                var doctorId = doctors.FirstOrDefault()?.Id;

                context.Diagnoses.AddRange(
                    new Diagnosis
                    {
                        VisitId = visits[0].Id,
                        DiagnosisCode = "I10",
                        DiagnosisName = "Essential hypertension",
                        DiagnosisType = DiagnosisType.Chronic,
                        Notes = "Stage 1, lifestyle changes advised",
                        DiagnosedByDoctorId = doctorId,
                        DiagnosedAtUtc = now
                    },
                    new Diagnosis
                    {
                        VisitId = visits.Count > 1 ? visits[1].Id : visits[0].Id,
                        DiagnosisCode = "R51",
                        DiagnosisName = "Headache, unspecified",
                        DiagnosisType = DiagnosisType.Primary,
                        Notes = "Investigate if it persists",
                        DiagnosedByDoctorId = doctorId,
                        DiagnosedAtUtc = now
                    }
                );

                await context.SaveChangesAsync();
            }

            // ---------------- Prescriptions ----------------
            if (!await context.Prescriptions.AnyAsync() && visits.Count > 0 && medications.Count > 0)
            {
                var prescription = new Prescription
                {
                    VisitId = visits[0].Id,
                    PrescribedByDoctorId = doctors.FirstOrDefault()?.Id ?? 0,
                    Status = PrescriptionStatus.Active,
                    IssuedAtUtc = now,
                    Notes = "Continue for one month then review",
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                context.Prescriptions.Add(prescription);
                await context.SaveChangesAsync();

                context.PrescriptionItems.AddRange(
                    new PrescriptionItem
                    {
                        PrescriptionId = prescription.Id,
                        MedicationId = medications.FirstOrDefault(m => m.GenericName.Contains("Amlodipine", StringComparison.OrdinalIgnoreCase))?.Id ?? medications[0].Id,
                        Dose = "1 tablet",
                        Frequency = "Once daily",
                        DurationDays = 30,
                        Route = "Oral",
                        Quantity = 30m,
                        Instructions = "Take in the morning before breakfast"
                    },
                    new PrescriptionItem
                    {
                        PrescriptionId = prescription.Id,
                        MedicationId = medications.FirstOrDefault(m => m.GenericName.Contains("Paracetamol", StringComparison.OrdinalIgnoreCase))?.Id ?? medications[0].Id,
                        Dose = "1 tablet",
                        Frequency = "Twice daily when needed",
                        DurationDays = 10,
                        Route = "Oral",
                        Quantity = 20m,
                        Instructions = "Maximum 2 tablets per day"
                    }
                );

                await context.SaveChangesAsync();
            }

            // ---------------- Reviews ----------------
            if (!await context.Reviews.AnyAsync())
            {
                var reviewedAppointment = appointments.FirstOrDefault(a => a.Status == AppointmentStatus.Completed);
                if (reviewedAppointment is not null)
                {
                    context.Reviews.Add(new Review
                    {
                        AppointmentId = reviewedAppointment.Id,
                        DoctorRating = 5,
                        ClinicRating = 4,
                        StaffRating = 5,
                        Comment = "Very professional doctor and friendly reception team.",
                        IsAnonymous = false,
                        ReviewedAtUtc = now
                    });

                    await context.SaveChangesAsync();
                }
            }

            // ---------------- Notifications ----------------
            if (!await context.Notifications.AnyAsync() && patients.Count > 0)
            {
                var firstAppointment = appointments.FirstOrDefault();

                context.Notifications.AddRange(
                    new Notification
                    {
                        UserId = patients[0].UserId ?? patientUserOne?.Id ?? string.Empty,
                        AppointmentId = firstAppointment?.Id,
                        NotificationType = "AppointmentReminder",
                        Channel = NotificationChannel.InApp,
                        Title = "Upcoming appointment",
                        Message = "You have an appointment scheduled tomorrow. Please arrive 10 minutes early.",
                        Status = NotificationStatus.Read,
                        SentAtUtc = now,
                        ReadAtUtc = now,
                        CreatedAtUtc = now
                    },
                    new Notification
                    {
                        UserId = patients[1].UserId ?? patientUserTwo?.Id ?? string.Empty,
                        AppointmentId = firstAppointment?.Id,
                        NotificationType = "AppointmentConfirmation",
                        Channel = NotificationChannel.InApp,
                        Title = "Appointment confirmed",
                        Message = "Your appointment has been confirmed by the clinic.",
                        Status = NotificationStatus.Sent,
                        SentAtUtc = now,
                        CreatedAtUtc = now
                    }
                );

                await context.SaveChangesAsync();
            }

            // ---------------- Notification preferences ----------------
            if (!await context.NotificationPreferences.AnyAsync())
            {
                var preferences = new List<NotificationPreference>();

                if (patientUserOne is not null)
                    preferences.Add(BuildPreference(patientUserOne.Id, now));
                if (patientUserTwo is not null)
                    preferences.Add(BuildPreference(patientUserTwo.Id, now));

                context.NotificationPreferences.AddRange(preferences);
                await context.SaveChangesAsync();
            }

            logger.LogInformation("Clinic seed data is ready.");
        }

        private static Staff BuildStaff(
            User user,
            string employeeNo,
            string firstName,
            string lastName,
            string email,
            string phone,
            string jobTitle,
            EntityCodeType codeType,
            int clinicId,
            DateTime hireDate,
            DateTime now)
        {
            return new Staff
            {
                UserId = user.Id,
                EmployeeNo = employeeNo,
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Email = email,
                JobTitle = jobTitle,
                PrimaryClinicId = clinicId,
                Status = StaffStatus.Active,
                HireDate = hireDate,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
        }

        private static NotificationPreference BuildPreference(string userId, DateTime now)
        {
            return new NotificationPreference
            {
                UserId = userId,
                AllowInApp = true,
                AllowEmail = true,
                AllowSMS = false,
                AllowPush = true,
                AllowWhatsApp = false,
                AppointmentReminders = true,
                GeneralUpdates = true,
                UpdatedAtUtc = now
            };
        }

        private static async Task<User?> EnsureUserAsync(
            UserManager<User> userManager,
            string email,
            string fullName,
            string password,
            string role,
            string phone,
            ILogger logger)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                if (!await userManager.IsInRoleAsync(existing, role))
                    await userManager.AddToRoleAsync(existing, role);

                return existing;
            }

            var user = new User
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                PhoneNumber = phone,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Could not seed user {Email}: {Errors}", email,
                    string.Join("; ", result.Errors.Select(e => e.Description)));
                return null;
            }

            await userManager.AddToRoleAsync(user, role);
            return user;
        }
    }
}
