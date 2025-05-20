using ExamiNation.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamiNation.Application.Validators.Auth
{
    public class LoginModelDtoValidator : AbstractValidator<LoginModelDto>
    {
        public LoginModelDtoValidator()
        {
            RuleFor(x => x)
               .NotNull().WithMessage("LoginModelDto cannot be null.");

            RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.")
           .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");


            RuleFor(x => x.Password)
             .NotEmpty().WithMessage("Password is required.");
        }
    }
}
