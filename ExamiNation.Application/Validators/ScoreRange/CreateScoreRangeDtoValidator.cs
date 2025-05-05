using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;
using ScoreRangeEntity = ExamiNation.Domain.Entities.Test.ScoreRange;
namespace ExamiNation.Application.Validators.ScoreRange
{
    public class CreateScoreRangeDtoValidator : AbstractValidator<CreateScoreRangeDto>
    {
        private readonly IScoreRangeRepository _scoreRangeRepository;

        public CreateScoreRangeDtoValidator(IScoreRangeRepository scoreRangeRepository)
        {
            _scoreRangeRepository = scoreRangeRepository;

            RuleFor(x => x.TestId)
                .NotEmpty().WithMessage("TestId is required.");

            RuleFor(x => x.MinScore)
                .LessThanOrEqualTo(x => x.MaxScore)
                .WithMessage("MinScore must be less than or equal to MaxScore.");

            RuleFor(x => x.Classification)
                .NotEmpty().WithMessage("Classification is required.")
                .MaximumLength(100).WithMessage("Classification can't exceed 100 characters.");

            RuleFor(x => x)
                .MustAsync(NotOverlapWithExistingRanges)
                .WithMessage("The score range overlaps with an existing range for this test.");
        }

        private async Task<bool> NotOverlapWithExistingRanges(CreateScoreRangeDto dto, CancellationToken cancellationToken)
        {
            QueryOptions<ScoreRangeEntity> options = new QueryOptions<ScoreRangeEntity> { Filter = l => l.TestId == dto.TestId };


            var existingRanges = await _scoreRangeRepository.GetAllAsync();

            return existingRanges.All(range =>
                dto.MaxScore < range.MinScore || dto.MinScore > range.MaxScore
            );
        }

        public override ValidationResult Validate(ValidationContext<CreateScoreRangeDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }

}
