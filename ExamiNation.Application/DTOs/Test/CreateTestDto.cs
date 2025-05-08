using ExamiNation.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Test
{
    public class CreateTestDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public TestType Type { get; set; }

        public IFormFile ImageUrl { get; set; }

    }
}
