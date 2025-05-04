using ExamiNation.Application.DTOs.TestResult;
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

            RuleFor(x => x.CompletedAt)
              .GreaterThan(x => x.StartedAt)
              .When(x => x.StartedAt.HasValue && x.CompletedAt.HasValue)
              .WithMessage("CompletedAt must be after StartedAt.");

        }
    }


}
