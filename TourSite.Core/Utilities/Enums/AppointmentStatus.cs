using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Utilities.Enums
{
    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        CheckedIn,
        InProgress,
        Completed,
        Cancelled,
        NoShow,
        Rescheduled
    }
}
