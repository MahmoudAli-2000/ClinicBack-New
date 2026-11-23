
namespace TourSite.Core.DTOs.Visits
{
    public class VitalSignDto
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? HeightCm { get; set; }
        public short? SystolicBP { get; set; }
        public short? DiastolicBP { get; set; }
        public short? HeartRateBpm { get; set; }
        public decimal? TemperatureC { get; set; }
        public decimal? OxygenSaturationPct { get; set; }
        public short? RespiratoryRate { get; set; }
        public int? RecordedByStaffId { get; set; }
        public DateTime RecordedAtUtc { get; set; }
        public string? Notes { get; set; }
    }
}
