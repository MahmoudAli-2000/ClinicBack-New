namespace TourSite.Core.Utilities
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Receptionist = "Receptionist";
        public const string Nurse = "Nurse";
        public const string Manager = "Manager";
        public const string Patient = "Patient";

        public static readonly string[] All =
        {
            Admin, Doctor, Receptionist, Nurse, Manager, Patient
        };

        public const string AdminManager = Admin + "," + Manager;
        public const string FrontDesk = Admin + "," + Manager + "," + Receptionist;
        public const string ClinicalStaff = Admin + "," + Manager + "," + Doctor + "," + Nurse;
        public const string Staff = Admin + "," + Manager + "," + Doctor + "," + Receptionist + "," + Nurse;
    }
}
