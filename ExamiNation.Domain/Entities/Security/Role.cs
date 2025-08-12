using Microsoft.AspNetCore.Identity;

namespace ExamiNation.Domain.Entities.Security
{
    public class Role : IdentityRole<Guid>
    {
        private string Description { get; set; }
    }
}
