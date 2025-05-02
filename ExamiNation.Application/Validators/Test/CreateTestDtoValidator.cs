﻿using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Nito.AsyncEx;

namespace ExamiNation.Application.Validators.Test
{
    public class CreateTestDtoValidator : AbstractValidator<CreateTestDto>
    {
        private readonly ITestRepository _testRepository;

        public CreateTestDtoValidator(ITestRepository testRepository)
        {
            _testRepository = testRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.")
                .MustAsync(NameMustBeUnique).WithMessage("A test with the same name already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid test type.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid test status.");

            RuleFor(x => x.CompletedAt)
                .GreaterThan(x => x.StartedAt)
                .When(x => x.StartedAt.HasValue && x.CompletedAt.HasValue)
                .WithMessage("CompletedAt must be after StartedAt.");
        }

        private async Task<bool> NameMustBeUnique(CreateTestDto dto, string name, CancellationToken cancellation)
        {
            var existingTest = await _testRepository.FindFirstTestEntityAsync(t => t.Name == name);
            return existingTest == null;
        }

        public override ValidationResult Validate(ValidationContext<CreateTestDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}
