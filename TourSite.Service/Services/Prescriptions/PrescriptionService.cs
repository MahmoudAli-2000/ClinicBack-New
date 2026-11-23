using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TourSite.Core;
using TourSite.Core.DTOs.Prescriptions;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Prescriptions
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _currentUser;

        public PrescriptionService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<PrescriptionDto> CreateAsync(PrescriptionCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can create prescriptions.");

            var doctorId = await GetDoctorIdForCurrentUserAsync(cancellationToken);
            if (!_currentUser.IsAdmin && doctorId is null)
                throw new AppException(403, "Only doctors can prescribe medications.");

            return await _unitOfWork.ExecuteInTransactionAsync(
                async () => await CreateInternalAsync(dto, doctorId!.Value, cancellationToken),
                IsolationLevel.ReadCommitted,
                cancellationToken);
        }

        private async Task<PrescriptionDto> CreateInternalAsync(PrescriptionCreateDto dto, int doctorId, CancellationToken ct)
        {
            var visit = await _unitOfWork.Repository<Visit>().Query()
                .AsNoTracking()
                .Include(v => v.Appointment)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(v => v.Id == dto.VisitId, ct);
            if (visit is null)
                throw new AppException(404, "Visit not found.");

            if (!_currentUser.IsAdmin && _currentUser.IsDoctor)
            {
                var appointmentDoctorId = await _unitOfWork.Repository<Appointment>().Query()
                    .AsNoTracking()
                    .Where(a => a.Id == visit.AppointmentId)
                    .Select(a => a.DoctorClinic.DoctorId)
                    .FirstOrDefaultAsync(ct);
                if (appointmentDoctorId != doctorId)
                    throw new AppException(403, "You can only prescribe for your own patients.");
            }

            var existing = await _unitOfWork.Repository<Prescription>().Query()
                .AsNoTracking()
                .AnyAsync(p => p.VisitId == dto.VisitId, ct);
            if (existing)
                throw new AppException(409, "A prescription already exists for this visit.");

            var now = DateTime.UtcNow;
            var prescription = new Prescription
            {
                VisitId = dto.VisitId,
                PrescribedByDoctorId = doctorId,
                Status = PrescriptionStatus.Active,
                IssuedAtUtc = now,
                Notes = dto.Notes,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            await _unitOfWork.Repository<Prescription>().AddAsync(prescription, ct);

            prescription.Items = new List<PrescriptionItem>();
            foreach (var itemDto in dto.Items)
            {
                var medication = await _unitOfWork.Repository<Medication>().Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == itemDto.MedicationId, ct);
                if (medication is null)
                    throw new AppException(404, $"Medication with ID {itemDto.MedicationId} not found.");

                if (!medication.IsActive)
                    throw new AppException(400, $"Medication '{medication.GenericName}' is not active.");

                prescription.Items.Add(_mapper.Map<PrescriptionItem>(itemDto));
            }

            _unitOfWork.Repository<Prescription>().Update(prescription);
            await _unitOfWork.CompleteAsync(ct);

            await _unitOfWork.Repository<Prescription>().Query()
                .FirstOrDefaultAsync(p => p.Id == prescription.Id, ct);
            return await GetByIdAsync(prescription.Id, ct);
        }

        public async Task<PrescriptionDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.Repository<Prescription>().Query()
                .AsNoTracking()
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Medication)
                .Include(p => p.PrescribedByDoctor)
                    .ThenInclude(d => d.Staff)
                .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                        .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (prescription is null)
                throw new AppException(404, "Prescription not found.");

            await VerifyPatientAccessAsync(prescription.Visit.Appointment.PatientId, cancellationToken);

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        private async Task VerifyPatientAccessAsync(int patientId, CancellationToken ct)
        {
            if (_currentUser.IsAdmin || _currentUser.IsClinicalStaff)
                return;

            if (_currentUser.IsPatient)
            {
                var patientUserId = await _unitOfWork.Repository<Patient>().Query()
                    .AsNoTracking()
                    .Where(p => p.Id == patientId)
                    .Select(p => p.UserId)
                    .FirstOrDefaultAsync(ct);
                if (patientUserId != _currentUser.UserId)
                    throw new AppException(403, "You do not have access to this prescription.");
            }
        }

        public async Task<IReadOnlyList<PrescriptionDto>> GetPatientHistoryAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Repository<Patient>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);
            if (patient is null)
                throw new AppException(404, "Patient not found.");

            if (_currentUser.IsPatient && patient.UserId != _currentUser.UserId)
                throw new AppException(403, "You can only view your own prescriptions.");

            var prescriptions = await _unitOfWork.Repository<Prescription>().Query()
                .AsNoTracking()
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Medication)
                .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                .Where(p => p.Visit.Appointment.PatientId == patientId)
                .OrderByDescending(p => p.IssuedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<PrescriptionDto>>(prescriptions);
        }

        public async Task<IReadOnlyList<PrescriptionDto>> GetDoctorHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
        {
            var doctorExists = await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .AnyAsync(d => d.Id == doctorId, cancellationToken);
            if (!doctorExists)
                throw new AppException(404, "Doctor not found.");

            var prescriptions = await _unitOfWork.Repository<Prescription>().Query()
                .AsNoTracking()
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Medication)
                .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                .Where(p => p.PrescribedByDoctorId == doctorId)
                .OrderByDescending(p => p.IssuedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<PrescriptionDto>>(prescriptions);
        }

        public async Task<PrescriptionDto> UpdateStatusAsync(int id, PrescriptionStatus status, CancellationToken cancellationToken = default)
        {
            if (!_currentUser.IsClinicalStaff)
                throw new AppException(403, "Only clinical staff can update prescription status.");

            var prescription = await _unitOfWork.Repository<Prescription>().Query()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (prescription is null)
                throw new AppException(404, "Prescription not found.");

            if (prescription.Status == PrescriptionStatus.Completed && status != PrescriptionStatus.Completed)
                throw new AppException(409, "Cannot modify a completed prescription.");

            prescription.Status = status;
            prescription.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Repository<Prescription>().Update(prescription);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        private async Task<int?> GetDoctorIdForCurrentUserAsync(CancellationToken ct)
        {
            if (_currentUser.UserId is null)
                return null;

            return await _unitOfWork.Repository<Doctor>().Query()
                .AsNoTracking()
                .Where(d => d.Staff.UserId == _currentUser.UserId)
                .Select(d => d.Id)
                .FirstOrDefaultAsync(ct);
        }
    }
}





