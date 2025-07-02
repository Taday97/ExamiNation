using ExamiNation.Domain.Entities.Test;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Option
{
    public class CreateOptionDto
    {

        [Required, StringLength(300)]
        public string Text { get; set; }

        [DefaultValue(false)]
        public bool IsCorrect { get; set; } = false;

        public Guid? QuestionId { get; set; }
    }
}
