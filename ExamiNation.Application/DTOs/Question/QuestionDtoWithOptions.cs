using ExamiNation.Application.DTOs.Option;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionDtoWithOptions : QuestionDto
    {
        public List<QuestionOptionDto>? Options { get; set; }
    }

}
