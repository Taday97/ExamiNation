using ExamiNation.Application.DTOs.Test;
using FluentValidation;

namespace ExamiNation.Application.Validators.Test
{
    public class TestDtoValidator : AbstractValidator<TestDto>
    {
        public TestDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid test type.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid test status.");

            RuleFor(x => x.CompletedAt)
                .GreaterThan(x => x.StartedAt)
                .When(x => x.StartedAt.HasValue && x.CompletedAt.HasValue)
                .WithMessage("CompletedAt must be after StartedAt.");
        }
    }
}
