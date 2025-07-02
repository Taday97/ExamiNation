using ExamiNation.Application.DTOs.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.Question
{
    public class QuestionDetailsDto : QuestionDto
    {
        public Guid? CognitiveCategoryId { get; set; }
        public string? CognitiveCategoryName { get; set; }
        public List<OptionDto>? Options { get; set; }

        public Guid? SelectedOptionId { get; set; }

    }

}
