using ExamiNation.Application.DTOs.Option;
using FluentValidation;

namespace ExamiNation.Application.Validators.Option
{
    public class CreateOptionDtoValidator : AbstractValidator<CreateOptionDto>
    {

        public CreateOptionDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text is required.")
                .Length(3, 50).WithMessage("Text must be between 3 and 50 characters.");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required.");
        }
       
    }

}
