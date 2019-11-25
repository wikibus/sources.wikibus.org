using System.Security.Claims;
using NullGuard;

namespace Wikibus.Common
{
    public static class PermissionsExtensions
    {
        public static bool HasPermission([AllowNull] this ClaimsPrincipal user, string permission)
        {
            return user?.HasClaim("permissions", permission) == true;
        }
    }
}
