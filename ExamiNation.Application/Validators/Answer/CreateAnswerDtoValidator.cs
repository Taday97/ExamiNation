using ExamiNation.Application.DTOs.Answer;
using FluentValidation;

namespace ExamiNation.Application.Validators.Answer
{
    public class CreateAnswerDtoValidator : AbstractValidator<CreateAnswerDto>
    {
        public CreateAnswerDtoValidator()
        {

            RuleFor(x => x.TestResultId)
                .NotEmpty().WithMessage("TestResult ID is required.")
                .Must(BeAValidGuid).WithMessage("TestResult ID must be a valid GUID.");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("Question ID is required.")
                .Must(BeAValidGuid).WithMessage("Question ID must be a valid GUID.");


            RuleFor(x => x.OptionId)
                .Must(BeAValidGuid).When(x => x.OptionId.HasValue).WithMessage("Option ID must be a valid GUID.");

        }

        private bool BeAValidGuid(Guid? id)
        {
            return id != Guid.Empty;
        }

        private bool BeAValidGuid(Guid id)
        {
            return id != Guid.Empty;
        }
    }
}
