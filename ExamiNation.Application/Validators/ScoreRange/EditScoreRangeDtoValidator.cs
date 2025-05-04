using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;

namespace ExamiNation.Application.Validators.ScoreRange
{
    public class EditScoreRangeDtoValidator : AbstractValidator<EditScoreRangeDto>
    {
        private readonly IScoreRangeRepository _scoreRangeRepository;

        public EditScoreRangeDtoValidator(IScoreRangeRepository scoreRangeRepository)
        {
            _scoreRangeRepository = scoreRangeRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("TestId is required.");

            RuleFor(x => x.MinScore)
                .LessThanOrEqualTo(x => x.MaxScore)
                .WithMessage("MinScore must be less than or equal to MaxScore.");

            RuleFor(x => x.Classification)
                .NotEmpty().WithMessage("Classification is required.")
                .MaximumLength(100).WithMessage("Classification can't exceed 100 characters.");

            RuleFor(x => x)
                .MustAsync(NotOverlapWithOtherRanges)
                .WithMessage("The score range overlaps with an existing range for this test.");
        }

        private async Task<bool> NotOverlapWithOtherRanges(EditScoreRangeDto dto, CancellationToken cancellationToken)
        {
            var existingRanges = await _scoreRangeRepository.GetScoreRangesAsync(l => l.TestId == dto.TestId);

            return existingRanges
                .Where(r => r.Id != dto.Id)
                .All(range =>
                    dto.MaxScore < range.MinScore || dto.MinScore > range.MaxScore
                );
        }

        public override ValidationResult Validate(ValidationContext<EditScoreRangeDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
