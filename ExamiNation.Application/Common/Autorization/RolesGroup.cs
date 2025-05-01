using ExamiNation.Application.Common.Authorization;
using ExamiNation.Domain.Enums;

namespace ExamiNation.Application.Common.Autorization
{
    public static class RoleGroups
    {
        public const string AdminOrDev = $"{RoleNames.Admin},{RoleNames.Developer}";
        public const string AdminOrDevOrCreator = $"{RoleNames.Admin},{RoleNames.Developer},{RoleNames.TestCreator}";

    }

}
