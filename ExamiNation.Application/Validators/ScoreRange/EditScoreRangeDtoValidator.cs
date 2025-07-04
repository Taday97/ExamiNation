using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;
using ScoreRangeEntity=ExamiNation.Domain.Entities.Test.ScoreRange;

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

            RuleFor(x => x)
               .MustAsync(NotDuplicateClassification)
               .WithMessage("The classification already exists for this test.");
        }

        private async Task<bool> NotOverlapWithOtherRanges(EditScoreRangeDto dto, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<ScoreRangeEntity>
            {
                Filter = l => l.TestId == dto.TestId,
            };
            var existingRanges = await _scoreRangeRepository.GetAllAsync(options);

            return existingRanges
                .Where(r => r.Id != dto.Id)
                .All(range =>
                    dto.MaxScore < range.MinScore || dto.MinScore > range.MaxScore
                );
        }
        private async Task<bool> NotDuplicateClassification(EditScoreRangeDto dto, CancellationToken cancellationToken)
        {
            QueryOptions<ScoreRangeEntity> options = new QueryOptions<ScoreRangeEntity> { Filter = l => l.TestId == dto.TestId };

            var existingRanges = await _scoreRangeRepository.GetAllAsync(options);

            return existingRanges.Where(r => r.Id != dto.Id).All(range =>
                !string.Equals(range.Classification, dto.Classification, StringComparison.OrdinalIgnoreCase)
            );
        }

        public override ValidationResult Validate(ValidationContext<EditScoreRangeDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }

    }
}
