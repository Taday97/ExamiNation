using ExamiNation.Domain.Enums;
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

        public virtual ICollection<Option> Options { get;  set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }


}
