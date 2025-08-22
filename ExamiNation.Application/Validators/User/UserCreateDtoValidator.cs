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
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {

        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleRepository _roleRepository;

        public UserCreateDtoValidator(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;


            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MustAsync(NameMustBeUnique).WithMessage("A user with the name already exists.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleForEach(x => x.Roles)
              .MustAsync(RoleMustExist).WithMessage("Role '{PropertyValue}' does not exist.");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("New password is required.")
               .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
               .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
               .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
               .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
               .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");

        }
        private async Task<bool> NameMustBeUnique(UserCreateDto dto, string name, CancellationToken cancellation)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
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

            var value = (await _roleRepository.GetByIdAsync(id)) != null;
            return value;
        }

        public override ValidationResult Validate(ValidationContext<UserCreateDto> context)
        {
            return AsyncContext.Run(() => base.ValidateAsync(context));
        }



    }



}
