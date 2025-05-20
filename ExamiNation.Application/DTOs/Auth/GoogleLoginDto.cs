using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Auth
{
    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; }
    }
}
