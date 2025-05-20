using System.ComponentModel.DataAnnotations;

namespace ExamiNation.Application.DTOs.Option
{
    public class OptionQuestionDto
    {
        public string Id { get; set; }

        [Required, StringLength(300)]
        public string Text { get; set; }


    }
}
