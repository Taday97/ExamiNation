using ExamiNation.Application.DTOs.Test;
using ExamiNation.Domain.Enums;
using ExamiNation.Domain.Interfaces.Test;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
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
                .Must(value => Enum.IsDefined(typeof(TestType), value))
                .WithMessage("Invalid test type.");

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidImage).WithMessage("The file must be a valid image (jpg, jpeg, png, gif).")
                .When(x => x.ImageUrl != null); 
        }

        private async Task<bool> NameMustBeUnique(CreateTestDto dto, string name, CancellationToken cancellation)
        {
            var existingTest = await _testRepository.FindFirstAsync(t => t.Name == name);
            return existingTest == null;
        }

        private bool BeAValidImage(IFormFile imageUrl)
        {
            if (imageUrl == null || imageUrl.Length == 0) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileExtension = Path.GetExtension(imageUrl.FileName)?.ToLower();

            return allowedExtensions.Contains(fileExtension);
        }


        public override ValidationResult Validate(ValidationContext<CreateTestDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }
}

