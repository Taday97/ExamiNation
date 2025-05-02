using ExamiNation.Application.DTOs.Test;
using ExamiNation.Application.Validators.Option;
using FluentValidation;

namespace ExamiNation.Application.Validators.Test
{
    public class QuestionDtoValidator : AbstractValidator<QuestionDto>
    {
        public QuestionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Question ID is required.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid question type.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleFor(x => x.TestText)
                .NotEmpty().WithMessage("Test text is required.");

            RuleForEach(x => x.Options)
                .SetValidator(new OptionDtoValidator());
        }
    }
}
