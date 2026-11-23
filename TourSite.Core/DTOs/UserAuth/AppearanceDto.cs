using System.ComponentModel.DataAnnotations;
namespace TourSite.Core.DTOs.UserAuth;
public class AppearanceDto
{
 [RegularExpression("^(light|dark|system)$")] public string Mode { get; set; } = "light";
 [RegularExpression("^#[0-9a-fA-F]{6}$")] public string Accent { get; set; } = "#278575";
 [RegularExpression("^(comfortable|compact)$")] public string Density { get; set; } = "comfortable";
 [RegularExpression("^(rounded|soft|square)$")] public string Corners { get; set; } = "rounded";
 [RegularExpression("^(default|large)$")] public string TextSize { get; set; } = "default";
 [RegularExpression("^(system|serif|mono)$")] public string Font { get; set; } = "system";
}
