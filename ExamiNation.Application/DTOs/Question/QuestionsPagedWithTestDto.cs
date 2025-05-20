using ExamiNation.Application.DTOs.Responses;
using ExamiNation.Application.DTOs.Test;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionsPagedWithTestDto
    {
        public TestDto Test { get; set; }
        public PagedResponse<QuestionDtoWithOptions?> Questions { get; set; }
    }
}
