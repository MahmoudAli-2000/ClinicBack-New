namespace TourSite.Core.Servicies.Contract
{
    public interface ICurrentUserAccessor
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool IsAdmin { get; }
        bool IsDoctor { get; }
        bool IsPatient { get; }
        bool IsFrontDesk { get; }
        bool IsClinicalStaff { get; }
    }
}
