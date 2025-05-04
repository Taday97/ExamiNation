using ExamiNation.Application.DTOs.Answer;
using ExamiNation.Application.DTOs.Role;
using ExamiNation.Application.DTOs.TestResult;
using ExamiNation.Domain.Enums;
using FluentValidation;

namespace ExamiNation.Application.Validators.TestResult
{
    public class TestResultDtoValidator : AbstractValidator<TestResultDto>
    {
        public TestResultDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .Must(BeAValidGuid);

            RuleFor(x => x.TestId)
                .NotEmpty()
                .Must(BeAValidGuid);

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .IsInEnum();


            RuleFor(x => x.Answers)
                .NotNull()
                .Must(ContainAtLeastOneAnswer);
        }

        private bool BeAValidGuid(Guid id)
        {
            return id != Guid.Empty;
        }

        private bool ContainAtLeastOneAnswer(List<AnswerDto> answers)
        {
            return answers != null && answers.Any();
        }
    }

}
