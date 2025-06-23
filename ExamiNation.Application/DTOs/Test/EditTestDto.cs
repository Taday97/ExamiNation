using ExamiNation.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Test
{
    public class EditTestDto
    {
        [Key]
        public Guid Id { get;  set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public TestType Type { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
