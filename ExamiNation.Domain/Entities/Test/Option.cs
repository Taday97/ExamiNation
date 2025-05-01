using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Domain.Entities.Test
{
    public class Option
    {
        [Key]
        public Guid Id { get;  set; }

        [Required, StringLength(300)]
        public string Text { get;  set; }
       
        [DefaultValue(false)]
        public bool IsCorrect { get;  set; } = false;

        [Required]
        public Guid QuestionId { get;  set; }
        public Question Question { get;  set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }


}
