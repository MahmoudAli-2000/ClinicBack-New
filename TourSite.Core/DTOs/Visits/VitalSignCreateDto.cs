using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Visits
{
    public class VitalSignCreateDto
    {
        public decimal? WeightKg { get; set; }
        public decimal? HeightCm { get; set; }
        public short? SystolicBP { get; set; }
        public short? DiastolicBP { get; set; }
        public short? HeartRateBpm { get; set; }
        public decimal? TemperatureC { get; set; }
        public decimal? OxygenSaturationPct { get; set; }
        public short? RespiratoryRate { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
