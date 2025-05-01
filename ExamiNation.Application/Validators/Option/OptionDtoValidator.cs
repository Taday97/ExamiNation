using ExamiNation.Application.DTOs.Option;
using FluentValidation;

namespace ExamiNation.Application.Validators.Option
{
    public class OptionDtoValidator : AbstractValidator<OptionDto>
    {
        public OptionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().When(dto => !string.IsNullOrEmpty(dto.Id)).WithMessage("Option ID cannot be empty.")
                .Must((dto, id) => id == dto.Id).When(dto => !string.IsNullOrEmpty(dto.Id)).WithMessage("Option ID in request body does not match the ID in the URL.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text is required.")
                .Length(3, 50).WithMessage("Text must be between 3 and 50 characters.");

            RuleFor(x => x.QuestionId)
               .NotEmpty().WithMessage("QuestionId is required.");

        }
    }

}
