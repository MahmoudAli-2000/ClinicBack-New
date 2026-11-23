using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.DTOs.Notifications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Entities;
using TourSite.Core.Exceptions;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserAccessor _currentUser;
        private readonly INotificationChannelSender _channelSender;
        private readonly IMapper _mapper;

        public NotificationService(
            IUnitOfWork unitOfWork,
            ICurrentUserAccessor currentUser,
            INotificationChannelSender channelSender,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _channelSender = channelSender;
            _mapper = mapper;
        }

        public async Task PublishAsync(string userId, string type, string title, string message, int? appointmentId = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Repository<User>().Query()
                .AsNoTracking()
                .Include(u => u.NotificationPreference)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
                return;

            var prefs = user.NotificationPreference ?? new NotificationPreference();
            if (type == "AppointmentReminder" && !prefs.AppointmentReminders) return;
            if (type.StartsWith("Appointment") && type != "AppointmentReminder" && !prefs.AppointmentUpdates) return;
            if (!type.StartsWith("Appointment") && !prefs.GeneralUpdates) return;
            if (!prefs.AllowInApp && !prefs.AllowEmail && !prefs.AllowSMS && !prefs.AllowPush && !prefs.AllowWhatsApp) return;
            var channel = DeterminePreferredChannel(type, prefs);

            var notification = new Notification
            {
                UserId = userId,
                NotificationType = type,
                Channel = channel,
                Title = title,
                Message = message,
                AppointmentId = appointmentId,
                Status = NotificationStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
                ScheduledAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            try { await _channelSender.SendAsync(notification, cancellationToken); notification.Status = NotificationStatus.Sent; notification.SentAtUtc=DateTime.UtcNow; }
            catch { notification.Status = NotificationStatus.Failed; }
            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<PageinationResponse<NotificationDto>> GetMineAsync(bool unreadOnly, NotificationQueryDto query, CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            var baseQuery = _unitOfWork.Repository<Notification>().Query()
                .AsNoTracking()
                .Where(n => n.UserId == _currentUser.UserId);

            baseQuery = query.Hidden ? baseQuery.Where(n => n.HiddenAtUtc != null) : baseQuery.Where(n => n.HiddenAtUtc == null);
            if(query.Category == "reminders") baseQuery = baseQuery.Where(n => n.NotificationType == "AppointmentReminder");
            if(query.Category == "appointments") baseQuery = baseQuery.Where(n => n.NotificationType.StartsWith("Appointment") && n.NotificationType != "AppointmentReminder");
            if(query.Category == "general") baseQuery = baseQuery.Where(n => !n.NotificationType.StartsWith("Appointment"));
            if (unreadOnly)
                baseQuery = baseQuery.Where(n => n.ReadAtUtc == null);

            var count = await baseQuery.CountAsync(cancellationToken);
            var notifications = await baseQuery
                .OrderByDescending(n => n.CreatedAtUtc)
                .Skip((query.SafePageIndex - 1) * query.SafePageSize)
                .Take(query.SafePageSize)
                .ToListAsync(cancellationToken);

            return new PageinationResponse<NotificationDto>(query.SafePageIndex, query.SafePageSize, count,
                _mapper.Map<IEnumerable<NotificationDto>>(notifications));
        }

        public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            return await _unitOfWork.Repository<Notification>().Query()
                .AsNoTracking()
                .CountAsync(n => n.UserId == _currentUser.UserId && n.ReadAtUtc == null && n.HiddenAtUtc == null, cancellationToken);
        }

        public async Task MarkReadAsync(int id, CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            var notification = await _unitOfWork.Repository<Notification>().Query()
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            if (notification is null)
                throw new AppException(404, "Notification not found.");

            if (notification.UserId != _currentUser.UserId)
                throw new AppException(403, "You can only manage your own notifications.");

            notification.ReadAtUtc = DateTime.UtcNow;
            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        private async Task<Notification> OwnAsync(int id, CancellationToken ct) {
            if(_currentUser.UserId is null) throw new AppException(401,"Authentication required.");
            var item=await _unitOfWork.Repository<Notification>().Query().FirstOrDefaultAsync(n=>n.Id==id && n.UserId==_currentUser.UserId,ct);
            return item ?? throw new AppException(404,"Notification not found.");
        }
        public async Task SetHiddenAsync(int id, bool hidden, CancellationToken cancellationToken = default) {
            var item=await OwnAsync(id,cancellationToken);item.HiddenAtUtc=hidden ? DateTime.UtcNow : null;
            _unitOfWork.Repository<Notification>().Update(item);await _unitOfWork.CompleteAsync(cancellationToken);
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) {
            var item=await OwnAsync(id,cancellationToken);_unitOfWork.Repository<Notification>().Delete(item);await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            var unread = await _unitOfWork.Repository<Notification>().Query()
                .Where(n => n.UserId == _currentUser.UserId && n.ReadAtUtc == null && n.HiddenAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var n in unread)
            {
                n.ReadAtUtc = DateTime.UtcNow;
                _unitOfWork.Repository<Notification>().Update(n);
            }
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async Task<NotificationPreferenceDto> GetPreferencesAsync(CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            var prefs = await _unitOfWork.Repository<NotificationPreference>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken);

            if (prefs is null)
            {
                prefs = new NotificationPreference { UserId = _currentUser.UserId };
                await _unitOfWork.Repository<NotificationPreference>().AddAsync(prefs, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }

            return _mapper.Map<NotificationPreferenceDto>(prefs);
        }

        public async Task<NotificationPreferenceDto> UpdatePreferencesAsync(NotificationPreferenceDto dto, CancellationToken cancellationToken = default)
        {
            if (_currentUser.UserId is null)
                throw new AppException(401, "Authentication required.");

            var prefs = await _unitOfWork.Repository<NotificationPreference>().Query()
                .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId, cancellationToken);

            if (prefs is null)
            {
                prefs = new NotificationPreference { UserId = _currentUser.UserId };
                await _unitOfWork.Repository<NotificationPreference>().AddAsync(prefs, cancellationToken);
            }

            prefs.AllowInApp = dto.AllowInApp;
            prefs.AllowEmail = dto.AllowEmail;
            prefs.AllowSMS = dto.AllowSMS;
            prefs.AllowPush = dto.AllowPush;
            prefs.AllowWhatsApp = dto.AllowWhatsApp;
            prefs.AppointmentReminders = dto.AppointmentReminders;
            prefs.AppointmentUpdates = dto.AppointmentUpdates;
            prefs.GeneralUpdates = dto.GeneralUpdates;
            prefs.UpdatedAtUtc = DateTime.UtcNow;

            // Query returns a tracked entity; a newly added preference must remain Added.
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<NotificationPreferenceDto>(prefs);
        }

        public async Task ProcessRemindersAsync(CancellationToken cancellationToken = default)
        {
            var upcomingAppointments = await _unitOfWork.Repository<Appointment>().Query()
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.DoctorClinic)
                    .ThenInclude(dc => dc.Doctor)
                .Where(a => a.Status == AppointmentStatus.Scheduled &&
                           a.ScheduledStartAtUtc > DateTime.UtcNow &&
                           a.ScheduledStartAtUtc <= DateTime.UtcNow.AddHours(24) &&
                           a.ReminderSentAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var appointment in upcomingAppointments)
            {
                await PublishAsync(
                    appointment.Patient.UserId ?? string.Empty,
                    "AppointmentReminder",
                    "Appointment Reminder",
                    $"You have an appointment scheduled for {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} UTC.",
                    appointment.Id,
                    cancellationToken);

                await PublishAsync(
                    appointment.DoctorClinic.Doctor.Staff.UserId ?? string.Empty,
                    "AppointmentReminder",
                    "Upcoming Appointment",
                    $"You have an appointment at {appointment.ScheduledStartAtUtc:yyyy-MM-dd HH:mm} UTC.",
                    appointment.Id,
                    cancellationToken);

                appointment.ReminderSentAtUtc = DateTime.UtcNow;
                _unitOfWork.Repository<Appointment>().Update(appointment);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        private static NotificationChannel DeterminePreferredChannel(string type, NotificationPreference prefs)
        {
            if (type is "AppointmentReminder" && prefs.AllowEmail)
                return NotificationChannel.Email;
            if (prefs.AllowInApp)
                return NotificationChannel.InApp;
            if (prefs.AllowEmail)
                return NotificationChannel.Email;
            if (prefs.AllowSMS)
                return NotificationChannel.SMS;
            if (prefs.AllowPush)
                return NotificationChannel.Push;
            return NotificationChannel.InApp;
        }
    }
}






