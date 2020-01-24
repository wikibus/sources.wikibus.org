using System.Security.Claims;
using NullGuard;

namespace Wikibus.Common
{
    public static class PermissionsExtensions
    {
        public static bool HasPermission([AllowNull] this ClaimsPrincipal user, string permission)
        {
            return user?.HasClaim(Permissions.Claim, permission) == true;
        }
    }
}
