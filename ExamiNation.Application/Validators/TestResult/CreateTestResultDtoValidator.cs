using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Application.Validators.Answer;
using ExamiNation.Application.Validators.Option;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.TestResult
{
    public class CreateTestResultDtoValidator : AbstractValidator<CreateTestResultDto>
    {
        public CreateTestResultDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");


            RuleFor(x => x.CompletedAt)
               .GreaterThan(x => x.StartedAt)
               .When(x => x.StartedAt.HasValue && x.CompletedAt.HasValue)
               .WithMessage("CompletedAt must be after StartedAt.");

            When(x => x.Answers != null && x.Answers.Any(), () =>
            {
                RuleForEach(x => x.Answers).SetValidator(new CreateAnswerDtoValidator());
            });
        }
    }
}
