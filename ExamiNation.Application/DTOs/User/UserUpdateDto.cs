using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.User
{
    public class UserUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string? Password { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }

}
