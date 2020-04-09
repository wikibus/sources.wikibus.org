using System.Linq;
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

        [return: AllowNull]
        public static string GetNameClaim([AllowNull] this ClaimsPrincipal user)
        {
            return (from c in user?.Claims
                where c.Type == ClaimTypes.NameIdentifier
                select c.Value).SingleOrDefault();
        }
    }
}
