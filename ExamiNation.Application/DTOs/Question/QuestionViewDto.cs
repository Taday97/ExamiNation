using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionViewDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(500)]
        public string Text { get; set; }

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public string TestName { get; set; }

        public Guid? CognitiveCategoryId { get; set; }
        public string CognitiveCategoryName { get; set; }
        public string CognitiveCategoryCode { get; set; }
        public int? QuestionNumber { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; } = 1.0m;
    }
}
