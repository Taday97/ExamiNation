using ExamiNation.Application.DTOs.Question;
using ExamiNation.Application.Validators.Option;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;

namespace ExamiNation.Application.Validators.Question
{
    public class EditQuestionDtoValidator : AbstractValidator<EditQuestionDto>
    {
        private readonly IQuestionRepository _questionRepository;

        public EditQuestionDtoValidator(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Question ID is required.");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(500).WithMessage("Question text cannot exceed 500 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid question type.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("Test ID is required.");

            RuleForEach(x => x.Options).ChildRules(options =>
            {
                options.RuleFor(o => o.Text).NotEmpty().MaximumLength(300);
            });

            When(x => x.Type == QuestionType.MultipleChoice || x.Type == QuestionType.TrueFalse, () =>
            {
                RuleFor(x => x.Options)
                    .Must(options => options.Any(o => o.IsCorrect))
                    .WithMessage("At least one correct option is required for MultipleChoice or TrueFalse questions.");
            });

            When(x => x.QuestionNumber.HasValue, () =>
            {
                RuleFor(x => x.QuestionNumber)
                .CustomAsync(async (code, context, cancellation) =>
                {
                    var dto = context.InstanceToValidate; 
                    var isUniqueQuestion = await IsUniqueQuestionNumberAsync(dto, cancellation);
                    if (!isUniqueQuestion)
                    {
                        context.AddFailure("QuestionNumber", "Question number is already used in this test.");
                    }
                });
            });
        }

        private async Task<bool> IsUniqueQuestionNumberAsync(EditQuestionDto dto, CancellationToken cancellationToken)
        {
            return await _questionRepository.FindFirstAsync(l =>l.Id!=dto.Id && l.TestId == dto.TestId && l.QuestionNumber == dto.QuestionNumber.Value) == null;
        }

        public override ValidationResult Validate(ValidationContext<EditQuestionDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
