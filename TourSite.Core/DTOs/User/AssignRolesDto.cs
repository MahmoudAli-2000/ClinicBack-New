using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.User
{
    public class AssignRolesDto
    {
        [Required]
        public List<string> Roles { get; set; } = new();
    }
}
