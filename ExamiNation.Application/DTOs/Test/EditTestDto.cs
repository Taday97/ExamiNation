using ExamiNation.Domain.Enums;
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
        public string Description { get; set; }

        [Required]
        public TestType Type { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Required]
        public TestStatus Status { get; set; }
    }
}
