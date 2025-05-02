using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using System.ComponentModel;
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

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Required]
        public TestStatus Status { get; set; }
    }
}
