using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.DTOs.Option
{
    public class QuestionOptionDto
    {
        public string Id { get; set; }

        [Required, StringLength(300)]
        public string Text { get; set; }

        [DefaultValue(false)]
        public bool IsCorrect { get; set; } = false;

    }
}
