using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }

}
