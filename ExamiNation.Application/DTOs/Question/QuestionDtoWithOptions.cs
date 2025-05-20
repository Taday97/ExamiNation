using ExamiNation.Application.DTOs.Option;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionDtoWithOptions : QuestionDto
    {
        public List<OptionQuestionDto>? Options { get; set; }

        public Guid? SelectedOptionId { get; set; }

    }

}
