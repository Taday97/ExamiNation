using ExamiNation.Domain.Entities.Test;
using ExamiNation.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Test
{
    public class TestDto
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public TestType Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal MaxScore { get; set; }

        public string? ImageUrl { get; set; }

    }
}
