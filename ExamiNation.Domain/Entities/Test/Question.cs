using ExamiNation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamiNation.Domain.Entities.Test
{
    public class Question
    {
        [Key]
        public Guid Id { get;  set; }

        [Required, StringLength(500)]
        public string Text { get;  set; }

        [Required]
        public QuestionType Type { get;  set; }

        [Required]
        public Guid TestId { get;  set; }
        public Test Test { get;  set; }

        public Guid? CognitiveCategoryId { get; set; }
        public CognitiveCategory? CognitiveCategory { get; set; }
        public int? QuestionNumber { get; set; }

        [Required]
        [Precision(10, 4)]
        public decimal Score { get; set; } = 1.0m;

        public virtual ICollection<Option> Options { get; set; } = new List<Option>();
        public virtual ICollection<Answer> Answers { get;  set; } = new List<Answer>();

        [NotMapped]
        public string CognitiveCategoryCode { get; set; }
    }


}
