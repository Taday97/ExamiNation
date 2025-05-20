using ExamiNation.Application.DTOs.Auth;
using FluentValidation;

namespace ExamiNation.Application.Validators.Auth
{
    public class GoogleLoginDtoValidator : AbstractValidator<GoogleLoginDto>
    {
        public GoogleLoginDtoValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID token is required.");
        }
    }

}
