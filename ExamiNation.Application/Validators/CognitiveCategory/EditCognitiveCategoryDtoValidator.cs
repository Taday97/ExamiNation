using ExamiNation.Application.DTOs.CognitiveCategory;
using FluentValidation;

namespace ExamiNation.Application.Validators.CognitiveCategory
{
    public class EditCognitiveCategoryDtoValidator : AbstractValidator<EditCognitiveCategoryDto>
    {

        public EditCognitiveCategoryDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(Id => Guid.TryParse(Id.ToString(), out _)).WithMessage("CognitiveCategory ID must be a valid GUID.")
                .Must((dto, Id) => Id == dto.Id).When(dto => !string.IsNullOrEmpty(dto.Id.ToString())).WithMessage("CognitiveCategory ID in request body does not match the ID in the URL.");

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
