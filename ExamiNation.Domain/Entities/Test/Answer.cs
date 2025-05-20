using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamiNation.Domain.Entities.Test
{
    public class Answer
    {
        [Key]
        public Guid Id { get;  set; }

        [Required]
        public Guid TestResultId { get;  set; }
        public TestResult TestResult { get;  set; }

        [Required]
        public Guid QuestionId { get;  set; }
        public Question Question { get;  set; }

        public Guid? OptionId { get;  set; }
        public Option Option { get;  set; }

        [StringLength(1000)]
        public string? Text { get;  set; }
    }

}
