using ExamiNation.Application.DTOs.CognitiveCategory;
using ExamiNation.Application.DTOs.ScoreRange;
using ExamiNation.Domain.Common;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;
using CognitiveCategoryEntity = ExamiNation.Domain.Entities.Test.CognitiveCategory;

namespace ExamiNation.Application.Validators.CognitiveCategory
{
    public class CreateCognitiveCategoryDtoValidator : AbstractValidator<CreateCognitiveCategoryDto>
    {
        private readonly ICognitiveCategoryRepository _cognitiveCategoryRepository;

        public CreateCognitiveCategoryDtoValidator(ICognitiveCategoryRepository cognitiveCategoryRepository)
        {
            _cognitiveCategoryRepository = cognitiveCategoryRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");

            RuleFor(x => x.TestTypeId)
                .GreaterThan(0).WithMessage("A valid test type must be selected.");
            RuleFor(x => x)
                .MustAsync(NotDuplicateName)
                .WithMessage("Name already exists for this test type.");

            RuleFor(x => x)
                .MustAsync(NotDuplicateCode)
                .WithMessage("Code already exists for this test type.");
        }

        private async Task<bool> NotDuplicateName(CreateCognitiveCategoryDto dto, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<CognitiveCategoryEntity>
            {
                Filter = cc => cc.TestTypeId == dto.TestTypeId && cc.Name.ToLower() == dto.Name.ToLower() 
            };

            var existingItems = await _cognitiveCategoryRepository.GetAllAsync(options);
            return !existingItems.Any();
        }

        private async Task<bool> NotDuplicateCode(CreateCognitiveCategoryDto dto, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<CognitiveCategoryEntity>
            {
                Filter = cc => cc.TestTypeId == dto.TestTypeId && cc.Code.ToLower() == dto.Code.ToLower()
            };

            var existingItems = await _cognitiveCategoryRepository.GetAllAsync(options);
            return !existingItems.Any();
        }

        public override ValidationResult Validate(ValidationContext<CreateCognitiveCategoryDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
