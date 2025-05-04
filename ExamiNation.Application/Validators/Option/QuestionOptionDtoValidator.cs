using ExamiNation.Application.DTOs.Option;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.Option
{
    public class QuestionOptionDtoValidator : AbstractValidator<QuestionOptionDto>
    {

        public QuestionOptionDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text is required.")
                .Length(3, 50).WithMessage("Text must be between 3 and 50 characters.");

        }

    }
}
