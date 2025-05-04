using ExamiNation.Application.DTOs.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.Test
{
    public class EditTestDtoValidator : AbstractValidator<EditTestDto>
    {
        public EditTestDtoValidator()
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
        }

        public override ValidationResult Validate(ValidationContext<EditTestDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
