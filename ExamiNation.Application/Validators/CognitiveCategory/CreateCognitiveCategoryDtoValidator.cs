using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.DTOs.Option;
using FluentValidation;

namespace ExamiNation.Application.Validators.CognitiveCategory
{
    public class CreateCognitiveCategoryDtoValidator : AbstractValidator<CreateCognitiveCategoryDto>
    {

        public CreateCognitiveCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.TestTypeId)
                .GreaterThan(0).WithMessage("A valid test type must be selected.");
        }

    }
}
