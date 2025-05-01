using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Role
{
    public class EditRoleDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
