using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.Validators.Option;
using ExamiNation.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;

namespace ExamiNation.Application.Validators.Question
{
    public class EditQuestionDtoValidator : AbstractValidator<EditQuestionDto>
    {
        public EditQuestionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Question ID is required.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid question type.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            // Optional: validate options if they are provided
            When(x => x.Options != null && x.Options.Any(), () =>
            {
                RuleForEach(x => x.Options).SetValidator(new EditOptionDtoValidator());

                // Require at least one correct option for certain question types
                When(x => x.Type == QuestionType.MultipleChoice || x.Type == QuestionType.TrueFalse, () =>
                {
                    RuleFor(x => x.Options)
                        .Must(options => options.Any(o => o.IsCorrect))
                        .WithMessage("At least one correct option is required for MultipleChoice or TrueFalse questions.");
                });
            });
        }

        public override ValidationResult Validate(ValidationContext<EditQuestionDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
