using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Role
{
    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; }
    }
}
