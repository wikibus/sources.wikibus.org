using System.Linq;
using System.Security.Claims;
using Argolis.Hydra.Nancy;
using Nancy;
using Nancy.Security;
using NullGuard;

namespace Wikibus.Nancy
{
    public static class PermissionsExtensions
    {
        public static bool HasPermission([AllowNull] this ClaimsPrincipal user, string permission)
        {
            return user?.HasClaim("permissions", permission) == true;
        }

        public static void RequiresPermissions(this NancyModule module, params string[] permissions)
        {
            module.RequiresClaims(claim => claim.Type == "permissions" && permissions.Contains(claim.Value));
        }

        public static bool HasPermission(this NancyContextWrapper context, string permission)
        {
            return context?.Current?.CurrentUser?.HasPermission(permission) == true;
        }
    }
}
