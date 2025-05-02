using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Validators.Option;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.NewFolder
{
    public class CreateTestResultDtoValidator : AbstractValidator<CreateTestResultDto>
    {
        public CreateTestResultDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0).WithMessage("Score must be a non-negative number.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");

            When(x => x.Options != null && x.Options.Any(), (Action)(() =>
            {
                RuleForEach(x => x.Options).SetValidator(new EditOptionDtoValidator());
            }));
        }
    }
}
