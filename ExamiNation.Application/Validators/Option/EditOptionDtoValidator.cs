using ExamiNation.Application.DTOs.Option;
using FluentValidation;

namespace ExamiNation.Application.Validators.Option
{
    public class EditOptionDtoValidator : AbstractValidator<EditOptionDto>
    {

        public EditOptionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().When(dto => !string.IsNullOrEmpty(dto.Id)).WithMessage("Option ID cannot be empty.")
                .Must(Id => Guid.TryParse(Id, out _)).WithMessage("Option ID must be a valid GUID.")
                .Must((dto, Id) => Id == dto.Id).When(dto => !string.IsNullOrEmpty(dto.Id)).WithMessage("Option ID in request body does not match the ID in the URL.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text is required.")
                .Length(3, 50).WithMessage("Text must be between 3 and 50 characters.");
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("Id must be a valid GUID.");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required.");
        }
       
    }

}
