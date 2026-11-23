using System;
using System.Collections.Generic;
using System.Linq;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Utilities
{
    public static class ClinicRules
    {
        public static readonly AppointmentStatus[] BlockingAppointmentStatuses =
        {
            AppointmentStatus.Scheduled,
            AppointmentStatus.Confirmed,
            AppointmentStatus.CheckedIn,
            AppointmentStatus.InProgress
        };

        public static bool IsBlockingStatus(AppointmentStatus status)
            => BlockingAppointmentStatuses.Contains(status);

        public static bool CanTransition(AppointmentStatus from, AppointmentStatus to)
        {
            return from switch
            {
                AppointmentStatus.Scheduled => to is AppointmentStatus.Confirmed
                    or AppointmentStatus.Cancelled
                    or AppointmentStatus.Rescheduled
                    or AppointmentStatus.NoShow
                    or AppointmentStatus.CheckedIn,
                AppointmentStatus.Confirmed => to is AppointmentStatus.CheckedIn
                    or AppointmentStatus.Cancelled
                    or AppointmentStatus.Rescheduled
                    or AppointmentStatus.NoShow,
                AppointmentStatus.CheckedIn => to is AppointmentStatus.InProgress
                    or AppointmentStatus.Completed
                    or AppointmentStatus.Cancelled
                    or AppointmentStatus.NoShow,
                AppointmentStatus.InProgress => to is AppointmentStatus.Completed
                    or AppointmentStatus.Cancelled,
                _ => false
            };
        }

        public static bool RangesOverlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
            => startA < endB && startB < endA;

        public static bool TimeRangesOverlap(TimeSpan startA, TimeSpan endA, TimeSpan startB, TimeSpan endB)
            => startA < endB && startB < endA;

        public static byte ToScheduleDayOfWeek(DateTime utcDate)
        {
            // Entity convention: 1 = Monday ... 7 = Sunday
            return utcDate.DayOfWeek == DayOfWeek.Sunday ? (byte)7 : (byte)utcDate.DayOfWeek;
        }

        public static bool EffectiveWindowCovers(DateTime dateUtc, DateTime? from, DateTime? to)
        {
            var date = dateUtc.Date;
            if (from.HasValue && date < from.Value.Date) return false;
            if (to.HasValue && date > to.Value.Date) return false;
            return true;
        }

        public static IReadOnlyList<(TimeSpan Start, TimeSpan End)> BuildSlots(TimeSpan start, TimeSpan end, short slotMinutes)
        {
            var slots = new List<(TimeSpan, TimeSpan)>();
            if (slotMinutes <= 0 || end <= start) return slots;

            var cursor = start;
            var duration = TimeSpan.FromMinutes(slotMinutes);
            while (cursor + duration <= end)
            {
                slots.Add((cursor, cursor + duration));
                cursor += duration;
            }

            return slots;
        }

        public static bool IsValidRating(byte? rating) => rating is null or (>= 1 and <= 5);

        public static bool IsReviewEligible(AppointmentStatus status) => status == AppointmentStatus.Completed;

        public static bool IsValidVital(decimal? weightKg, decimal? heightCm, short? systolic, short? diastolic,
            short? heartRate, decimal? temperatureC, decimal? oxygen, short? respiratory)
        {
            if (weightKg is < 1 or > 500) return false;
            if (heightCm is < 20 or > 300) return false;
            if (systolic is < 50 or > 250) return false;
            if (diastolic is < 30 or > 150) return false;
            if (heartRate is < 20 or > 250) return false;
            if (temperatureC is < 30 or > 45) return false;
            if (oxygen is < 50 or > 100) return false;
            if (respiratory is < 5 or > 80) return false;
            return true;
        }

        public static int ClampPageSize(int pageSize, int defaultSize = 20, int maxSize = 100)
        {
            if (pageSize <= 0) return defaultSize;
            return pageSize > maxSize ? maxSize : pageSize;
        }

        public static int ClampPageIndex(int pageIndex) => pageIndex < 1 ? 1 : pageIndex;
    }
}
