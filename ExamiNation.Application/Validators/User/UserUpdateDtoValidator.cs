using ExamiNation.Application.DTOs.User;
using ExamiNation.Domain.Entities.Security;
using ExamiNation.Domain.Interfaces.Security;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Nito.AsyncEx;
using System.Security.Claims;


namespace ExamiNation.Application.Validators.User
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleRepository _roleRepository;

        public UserUpdateDtoValidator(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MustAsync(NameMustBeUnique).WithMessage("A user with the name already exists.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleForEach(x => x.Roles)
                .MustAsync(RoleMustExist).WithMessage("Role '{PropertyValue}' does not exist.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("New password must be at least 6 characters long.")
                .Matches(@"[A-Z]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one number.")
                .Matches(@"[\W]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one special character.");
        }

        private async Task<bool> NameMustBeUnique(UserUpdateDto dto, string name, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(dto.Id.ToString()) || !Guid.TryParse(dto.Id.ToString(), out var userId))
            {
                return false;
            }

            var existingUser = await _userRepository.FindFirstUserAsync(u =>
                u.NormalizedUserName == name.ToUpper() &&
                u.Id != userId
            );

            return existingUser == null;
        }

        private async Task<bool> RoleMustExist(string roleId, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(roleId) || !Guid.TryParse(roleId, out var id))
                return false;

            return await _roleRepository.GetByIdAsync(id) != null;
        }

        public override ValidationResult Validate(ValidationContext<UserUpdateDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }
    }

}
