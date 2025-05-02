using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Option;
using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Test
{
    public class EditTestResultDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TestId { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; }

        public TestResultStatus Status { get; set; }

        public List<EditAnswerDto>? Answers { get; set; }
    }

}
