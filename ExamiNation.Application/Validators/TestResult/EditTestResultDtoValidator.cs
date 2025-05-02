using ExamiNation.Application.DTOs.Test;
using FluentValidation;

namespace ExamiNation.Application.Validators.TestResult
{
    public class EditTestResultDtoValidator : AbstractValidator<EditTestResultDto>
    {
        public EditTestResultDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Test result ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleFor(x => x.Score)
                .GreaterThan(0).WithMessage("Score must be greater than zero.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");

        }
    }


}
